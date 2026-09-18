let lab = null;

function makeShader(gl, type, source) {
    const shader = gl.createShader(type);
    gl.shaderSource(shader, source);
    gl.compileShader(shader);
    if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS))
        throw new Error(gl.getShaderInfoLog(shader) || "Shader compilation failed.");
    return shader;
}

function makeProgram(gl) {
    const vertex = makeShader(gl, gl.VERTEX_SHADER, `
        attribute vec3 aPosition;
        uniform mat4 uViewProjection;
        uniform float uTime;
        void main() {
            float c = cos(uTime * 0.08);
            float s = sin(uTime * 0.08);
            vec3 p = aPosition;
            p = vec3(p.x * c - p.z * s, p.y, p.x * s + p.z * c);
            gl_Position = uViewProjection * vec4(p, 1.0);
        }
    `);
    const fragment = makeShader(gl, gl.FRAGMENT_SHADER, `
        precision mediump float;
        void main() {
            gl_FragColor = vec4(0.05, 0.91, 1.0, 0.9);
        }
    `);
    const program = gl.createProgram();
    gl.attachShader(program, vertex);
    gl.attachShader(program, fragment);
    gl.linkProgram(program);
    if (!gl.getProgramParameter(program, gl.LINK_STATUS))
        throw new Error(gl.getProgramInfoLog(program) || "Program link failed.");
    return program;
}

function perspective(fov, aspect, near, far) {
    const f = 1 / Math.tan(fov / 2);
    const nf = 1 / (near - far);
    return new Float32Array([
        f / aspect, 0, 0, 0,
        0, f, 0, 0,
        0, 0, (far + near) * nf, -1,
        0, 0, (2 * far * near) * nf, 0
    ]);
}

function multiply(a, b) {
    const out = new Float32Array(16);
    for (let c = 0; c < 4; c++)
        for (let r = 0; r < 4; r++)
            out[c * 4 + r] =
                a[0 * 4 + r] * b[c * 4 + 0] +
                a[1 * 4 + r] * b[c * 4 + 1] +
                a[2 * 4 + r] * b[c * 4 + 2] +
                a[3 * 4 + r] * b[c * 4 + 3];
    return out;
}

function lookAt(eye, center, up) {
    const z = normalize(sub(eye, center));
    const x = normalize(cross(up, z));
    const y = cross(z, x);
    return new Float32Array([
        x[0], y[0], z[0], 0,
        x[1], y[1], z[1], 0,
        x[2], y[2], z[2], 0,
        -dot(x, eye), -dot(y, eye), -dot(z, eye), 1
    ]);
}

function sub(a,b){ return [a[0]-b[0],a[1]-b[1],a[2]-b[2]]; }
function dot(a,b){ return a[0]*b[0]+a[1]*b[1]+a[2]*b[2]; }
function cross(a,b){ return [a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0]]; }
function normalize(a){ const n=Math.hypot(a[0],a[1],a[2])||1; return [a[0]/n,a[1]/n,a[2]/n]; }

function starshipLines(lineCount) {
    const maxSegments = Math.max(1, Math.floor(lineCount));
    const vertices = [];
    const add = (a,b) => { vertices.push(a[0],a[1],a[2],b[0],b[1],b[2]); };

    // Primary hull: elongated cruise-ship silhouette.
    add([-42,0,0],[42,0,0]);
    add([-42,6,0],[42,6,0]);
    add([-42,-6,0],[42,-6,0]);
    add([-42,0,-7],[-42,0,7]);
    add([42,0,-7],[42,0,7]);

    // Deck rings and longitudinal ribs define the test ship's architecture.
    for (let i=0;i<18;i++) {
        const x=-40+i*(80/17);
        const half=10-(Math.abs(x)/42)*4;
        add([x,-6,-7],[x,-6,7]);
        add([x,6,-7],[x,6,7]);
        add([x,-6,-7],[x,6,-7]);
        add([x,-6,7],[x,6,7]);
        add([x,-half,-7],[x,half,-7]);
        add([x,-half,7],[x,half,7]);
    }

    // Bridge, observation decks, engineering spine and paired service corridors.
    const levels=[-9,-4,0,4,9];
    for (const y of levels) {
        add([-24,y,-5],[24,y,-5]);
        add([-24,y,5],[24,y,5]);
    }
    for (let i=0;i<10;i++) {
        const x=-20+i*(40/9);
        add([x,-10,-5],[x,10,-5]);
        add([x,-10,5],[x,10,5]);
    }

    // Solar-system cruise geometry: long route rails and a few orbit rings.
    for (let i=0;i<24;i++) {
        const t=i/23, x=-70+t*140;
        const z=Math.sin(t*Math.PI*2)*18;
        add([x,12,z],[x+5,12,z+Math.cos(t*Math.PI*2)*4]);
    }
    for (let ring=0;ring<4;ring++) {
        const radius=18+ring*9;
        const segments=48;
        for(let i=0;i<segments;i++){
            const a=i/segments*Math.PI*2;
            const b=(i+1)/segments*Math.PI*2;
            add([radius*Math.cos(a),-15,radius*Math.sin(a)],[radius*Math.cos(b),-15,radius*Math.sin(b)]);
        }
    }

    // Scale the experiment without changing the semantic ship.
    const base=vertices.slice();
    while (vertices.length/6 < maxSegments) {
        const source=(vertices.length/6)%Math.max(1,base.length/6);
        const offset=(source*6);
        const wobble=(vertices.length/6)*0.0007;
        vertices.push(
            base[offset]+wobble, base[offset+1], base[offset+2],
            base[offset+3]+wobble, base[offset+4], base[offset+5]
        );
    }
    return new Float32Array(vertices.slice(0,maxSegments*6));
}

export function startStarshipLineLab(canvasId, lineCount) {
    stopStarshipLineLab();
    const canvas=document.getElementById(canvasId);
    if(!canvas) throw new Error("Starship line lab canvas not found.");
    const gl=canvas.getContext("webgl") || canvas.getContext("experimental-webgl");
    if(!gl) throw new Error("WebGL is unavailable in this browser.");

    const program=makeProgram(gl);
    const buffer=gl.createBuffer();
    const position=gl.getAttribLocation(program,"aPosition");
    const viewProjection=gl.getUniformLocation(program,"uViewProjection");
    const timeUniform=gl.getUniformLocation(program,"uTime");

    lab={gl,program,buffer,position,viewProjection,timeUniform,lineCount:Math.max(100,Math.floor(lineCount||2500)),frames:0,start:performance.now(),last:performance.now(),fps:0,frameMs:0,raf:0};

    function resize(){
        const dpr=Math.min(window.devicePixelRatio||1,2);
        const width=Math.max(1,Math.floor(canvas.clientWidth*dpr));
        const height=Math.max(1,Math.floor(canvas.clientHeight*dpr));
        if(canvas.width!==width||canvas.height!==height){canvas.width=width;canvas.height=height;}
        gl.viewport(0,0,width,height);
    }

    function rebuild(){
        const data=starshipLines(lab.lineCount);
        gl.bindBuffer(gl.ARRAY_BUFFER,buffer);
        gl.bufferData(gl.ARRAY_BUFFER,data,gl.DYNAMIC_DRAW);
        lab.vertexCount=data.length/3;
    }

    lab.setLineCount=(count)=>{lab.lineCount=Math.max(100,Math.floor(count));rebuild();};
    rebuild();

    function frame(now){
        if(!lab) return;
        resize();
        const elapsed=now-lab.last;
        lab.last=now;
        lab.frames++;
        if(now-lab.start>=500){
            lab.fps=lab.frames*1000/(now-lab.start);
            lab.frameMs=1000/(lab.fps||1);
            lab.frames=0;
            lab.start=now;
            canvas.dispatchEvent(new CustomEvent("starshipmetrics",{detail:{fps:lab.fps,frameMs:lab.frameMs,lines:lab.lineCount,vertices:lab.vertexCount}}));
        }

        gl.enable(gl.DEPTH_TEST);
        gl.clearColor(0.005,0.012,0.025,1);
        gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT);
        gl.useProgram(program);
        gl.bindBuffer(gl.ARRAY_BUFFER,buffer);
        gl.enableVertexAttribArray(position);
        gl.vertexAttribPointer(position,3,gl.FLOAT,false,0,0);

        const aspect=canvas.width/Math.max(1,canvas.height);
        const projection=perspective(Math.PI/3,aspect,0.1,500);
        const view=lookAt([115,72,115],[0,0,0],[0,1,0]);
        gl.uniformMatrix4fv(viewProjection,false,multiply(projection,view));
        gl.uniform1f(timeUniform,now*0.001);
        gl.drawArrays(gl.LINES,0,lab.vertexCount);

        lab.raf=requestAnimationFrame(frame);
    }
    window.addEventListener("resize",resize);
    frame(performance.now());
}

export function setStarshipLineCount(lineCount) {
    if(lab?.setLineCount) lab.setLineCount(lineCount);
}

export function stopStarshipLineLab() {
    if(lab){
        cancelAnimationFrame(lab.raf);
        lab.gl.deleteBuffer(lab.buffer);
        lab.gl.deleteProgram(lab.program);
        lab=null;
    }
}
