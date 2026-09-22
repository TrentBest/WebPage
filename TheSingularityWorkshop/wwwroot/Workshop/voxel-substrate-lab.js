let lab = null;

function shader(gl, type, source) {
    const s = gl.createShader(type);
    gl.shaderSource(s, source);
    gl.compileShader(s);
    if (!gl.getShaderParameter(s, gl.COMPILE_STATUS))
        throw new Error(gl.getShaderInfoLog(s) || "Shader compilation failed.");
    return s;
}

function program(gl) {
    const vs = shader(gl, gl.VERTEX_SHADER, `
        precision mediump float;
        attribute vec3 aPosition;
        attribute vec3 aNormal;
        uniform mat4 uViewProjection;
        uniform float uDimension;
        varying vec3 vNormal;
        varying vec3 vPosition;

        void main() {
            vec3 p = aPosition;
            p.z *= mix(0.02, 1.0, uDimension);
            vNormal = aNormal;
            vPosition = p;
            gl_Position = uViewProjection * vec4(p, 1.0);
        }
    `);

    const fs = shader(gl, gl.FRAGMENT_SHADER, `
        precision mediump float;
        varying vec3 vNormal;
        varying vec3 vPosition;

        void main() {
            vec3 n = normalize(vNormal);
            vec3 light = normalize(vec3(-0.35, 0.85, 0.55));
            float diffuse = max(dot(n, light), 0.0);
            float rim = pow(1.0 - max(dot(n, normalize(vec3(0.1, 0.35, 1.0))), 0.0), 2.0);

            vec3 base = vec3(0.08, 0.55, 0.68);
            float strata = 0.5 + 0.5 * sin(vPosition.y * 0.38 + vPosition.x * 0.05);
            base += vec3(0.08, 0.05, 0.02) * strata;

            gl_FragColor = vec4(base * (0.22 + diffuse * 0.82) + vec3(0.0, 0.35, 0.5) * rim, 1.0);
        }
    `);

    const p = gl.createProgram();
    gl.attachShader(p, vs);
    gl.attachShader(p, fs);
    gl.linkProgram(p);

    if (!gl.getProgramParameter(p, gl.LINK_STATUS))
        throw new Error(gl.getProgramInfoLog(p) || "Shader link failed.");

    return p;
}

function identity() {
    return new Float32Array([
        1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1
    ]);
}

function multiply(a, b) {
    const out = new Float32Array(16);
    for (let c = 0; c < 4; c++)
        for (let r = 0; r < 4; r++)
            out[c * 4 + r] =
                a[r] * b[c * 4] +
                a[4 + r] * b[c * 4 + 1] +
                a[8 + r] * b[c * 4 + 2] +
                a[12 + r] * b[c * 4 + 3];
    return out;
}

function perspective(fov, aspect, near, far) {
    const f = 1 / Math.tan(fov / 2);
    const nf = 1 / (near - far);
    return new Float32Array([
        f / aspect,0,0,0,
        0,f,0,0,
        0,0,(far + near) * nf,-1,
        0,0,(2 * far * near) * nf,0
    ]);
}

function orthographic(left, right, bottom, top, near, far) {
    return new Float32Array([
        2/(right-left),0,0,0,
        0,2/(top-bottom),0,0,
        0,0,-2/(far-near),0,
        -(right+left)/(right-left),
        -(top+bottom)/(top-bottom),
        -(far+near)/(far-near),1
    ]);
}

function sub(a,b) { return [a[0]-b[0],a[1]-b[1],a[2]-b[2]]; }
function dot(a,b) { return a[0]*b[0]+a[1]*b[1]+a[2]*b[2]; }
function cross(a,b) { return [a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0]]; }
function normalize(a) {
    const n = Math.hypot(a[0],a[1],a[2]) || 1;
    return [a[0]/n,a[1]/n,a[2]/n];
}
function lookAt(eye, center, up) {
    const z = normalize(sub(eye,center));
    const x = normalize(cross(up,z));
    const y = cross(z,x);
    return new Float32Array([
        x[0],y[0],z[0],0,
        x[1],y[1],z[1],0,
        x[2],y[2],z[2],0,
        -dot(x,eye),-dot(y,eye),-dot(z,eye),1
    ]);
}
function clamp01(v) { return Math.max(0, Math.min(1, v)); }
function smooth(v) { const t=clamp01(v); return t*t*(3-2*t); }

// Squirrel Noise 5: deterministic, random-access procedural entropy.
const SQUIRREL_NOISE_1 = 0xD2A80A3F;
const SQUIRREL_NOISE_2 = 0xA884F197;
const SQUIRREL_NOISE_3 = 0x6C736F4B;
const SQUIRREL_NOISE_4 = 0xB79F3ABB;
const SQUIRREL_NOISE_5 = 0x1B56C4F5;

function squirrelNoise5(position, seed=0) {
    let bits = position | 0;
    bits = Math.imul(bits, SQUIRREL_NOISE_1);
    bits = (bits + seed) | 0;
    bits ^= bits >>> 9;
    bits = (bits + SQUIRREL_NOISE_2) | 0;
    bits ^= bits >>> 11;
    bits = Math.imul(bits, SQUIRREL_NOISE_3);
    bits ^= bits >>> 13;
    bits = (bits + SQUIRREL_NOISE_4) | 0;
    bits ^= bits >>> 15;
    bits = Math.imul(bits, SQUIRREL_NOISE_5);
    bits ^= bits >>> 17;
    return bits >>> 0;
}

function squirrelNoise2D(x, z, seed=0) {
    const mixed = Math.imul(x | 0, 0x1f123bb5) ^ Math.imul(z | 0, 0x5f356495);
    return squirrelNoise5(mixed, seed) / 4294967296;
}

function squirrelSmooth2D(x, z, scale, seed=0) {
    const px = x / scale;
    const pz = z / scale;
    const x0 = Math.floor(px), z0 = Math.floor(pz);
    const tx = smooth(px - x0), tz = smooth(pz - z0);
    const a = squirrelNoise2D(x0, z0, seed);
    const b = squirrelNoise2D(x0 + 1, z0, seed);
    const c = squirrelNoise2D(x0, z0 + 1, seed);
    const d = squirrelNoise2D(x0 + 1, z0 + 1, seed);
    return a + (b-a)*tx + (c-a)*tz + (a-b-c+d)*tx*tz;
}

function createVolume(width, height, depth) {
    return {
        width,
        height,
        depth,
        cells: new Uint8Array(width * height * depth)
    };
}

function index(v, x, y, z) {
    return x + v.width * (y + v.height * z);
}

function inside(v, x, y, z) {
    return x >= 0 && x < v.width && y >= 0 && y < v.height && z >= 0 && z < v.depth;
}

function setVoxel(v, x, y, z, value) {
    if (inside(v,x,y,z))
        v.cells[index(v,x,y,z)] = value ? 1 : 0;
}

function occupied(v, x, y, z) {
    return inside(v,x,y,z) && v.cells[index(v,x,y,z)] !== 0;
}

function fillBox(v, x0, y0, z0, x1, y1, z1) {
    for (let z=Math.max(0,z0); z<Math.min(v.depth,z1); z++)
        for (let y=Math.max(0,y0); y<Math.min(v.height,y1); y++)
            for (let x=Math.max(0,x0); x<Math.min(v.width,x1); x++)
                v.cells[index(v,x,y,z)] = 1;
}

function clearSphere(v, cx, cy, cz, radius) {
    const r2 = radius * radius;
    const x0=Math.floor(cx-radius), x1=Math.ceil(cx+radius);
    const y0=Math.floor(cy-radius), y1=Math.ceil(cy+radius);
    const z0=Math.floor(cz-radius), z1=Math.ceil(cz+radius);

    for(let z=z0;z<=z1;z++)
        for(let y=y0;y<=y1;y++)
            for(let x=x0;x<=x1;x++)
                if((x-cx)**2+(y-cy)**2+(z-cz)**2 <= r2)
                    setVoxel(v,x,y,z,0);
}

function clearCylinder(v, cx, cz, y0, y1, radius) {
    const r2=radius*radius;
    for(let z=Math.floor(cz-radius);z<=Math.ceil(cz+radius);z++)
        for(let x=Math.floor(cx-radius);x<=Math.ceil(cx+radius);x++)
            if((x-cx)**2+(z-cz)**2 <= r2)
                for(let y=Math.max(0,y0);y<Math.min(v.height,y1);y++)
                    setVoxel(v,x,y,z,0);
}

function retainEllipsoid(v, cx, cy, cz, rx, ry, rz) {
    for(let z=0;z<v.depth;z++)
        for(let y=0;y<v.height;y++)
            for(let x=0;x<v.width;x++) {
                const dx=(x-cx)/rx, dy=(y-cy)/ry, dz=(z-cz)/rz;
                if(dx*dx+dy*dy+dz*dz > 1)
                    v.cells[index(v,x,y,z)] = 0;
            }
}

function carveLine(v, a, b, radius) {
    const dx=b[0]-a[0], dy=b[1]-a[1], dz=b[2]-a[2];
    const length=Math.max(1,Math.hypot(dx,dy,dz));
    const steps=Math.ceil(length*1.4);

    for(let i=0;i<=steps;i++) {
        const t=i/steps;
        clearSphere(v,a[0]+dx*t,a[1]+dy*t,a[2]+dz*t,radius);
    }
}

function buildDetail() {
    const v=createVolume(72,72,72);
    v.cells.fill(1);

    // Start as a solid tower and scrape away everything outside the designed mass.
    retainEllipsoid(v,36,36,36,31,28,31);

    // Major voids.
    clearCylinder(v,36,36,0,72,8);
    carveLine(v,[8,18,8],[64,48,64],5);
    carveLine(v,[64,18,8],[8,48,64],4);

    // Repeating galleries / apertures.
    for(let i=0;i<7;i++) {
        const x=12+i*8;
        clearSphere(v,x,51,36,4.2);
        clearSphere(v,x,21,36,3.2);
    }

    // Fine vertical service shafts.
    for(let i=0;i<5;i++)
        clearCylinder(v,16+i*10,16+i*8,8,64,1.6);

    return v;
}

function buildMassive() {
    const w=144,h=52,d=144;
    const v=createVolume(w,h,d);
    v.cells.fill(1);

    // Scrape the sky away from a procedural landscape.
    for(let z=0;z<d;z++)
        for(let x=0;x<w;x++) {
            const continental = squirrelSmooth2D(x, z, 52, 17);
            const regional = squirrelSmooth2D(x, z, 18, 71);
            const local = squirrelNoise2D(x, z, 131);
            const wave = 7 + continental*15 + regional*8 + local*3;
            const height=Math.max(3,Math.floor(wave));
            for(let y=height;y<h;y++)
                v.cells[index(v,x,y,z)] = 0;
        }

    // Preserve large structures as remaining material in the carved landscape.
    const structures=[
        [18,18,34,34,38],
        [50,14,62,62,43],
        [92,22,106,106,48],
        [116,12,132,132,36],
        [70,62,76,76,32]
    ];

    for(const [x0,z0,x1,z1,top] of structures)
        fillBox(v,x0,1,z0,x1,Math.min(h,top),z1);

    // Cut avenues through the environment.
    for(let z=20;z<d;z+=28)
        clearCylinder(v,72,z,0,h,5);

    for(let x=24;x<w;x+=32)
        clearCylinder(v,x,72,0,h,4);

    return v;
}

function surfaceMesh(v) {
    const positions=[];
    const normals=[];
    let occupiedCount=0;
    let exposedFaces=0;

    const faces=[
        {n:[0,0,1],  corners:[[0,0,1],[1,0,1],[1,1,1],[0,1,1]]},
        {n:[0,0,-1], corners:[[1,0,0],[0,0,0],[0,1,0],[1,1,0]]},
        {n:[1,0,0],  corners:[[1,0,0],[1,0,1],[1,1,1],[1,1,0]]},
        {n:[-1,0,0], corners:[[0,0,1],[0,0,0],[0,1,0],[0,1,1]]},
        {n:[0,1,0],  corners:[[0,1,1],[1,1,1],[1,1,0],[0,1,0]]},
        {n:[0,-1,0], corners:[[0,0,0],[1,0,0],[1,0,1],[0,0,1]]}
    ];

    const tri=[0,1,2,0,2,3];

    for(let z=0;z<v.depth;z++)
        for(let y=0;y<v.height;y++)
            for(let x=0;x<v.width;x++) {
                if(!occupied(v,x,y,z)) continue;
                occupiedCount++;

                for(const face of faces) {
                    if(occupied(v,x+face.n[0],y+face.n[1],z+face.n[2]))
                        continue;

                    exposedFaces++;
                    for(const i of tri) {
                        const c=face.corners[i];
                        positions.push(
                            x+c[0]-v.width/2,
                            y+c[1]-v.height/2,
                            z+c[2]-v.depth/2
                        );
                        normals.push(face.n[0],face.n[1],face.n[2]);
                    }
                }
            }

    return {
        positions:new Float32Array(positions),
        normals:new Float32Array(normals),
        occupied:occupiedCount,
        exposed:exposedFaces
    };
}

function cameraMatrix(dimension, behavior, time, aspect, volume) {
    const d=clamp01(dimension);
    const cx=0, cy=0, cz=0;
    const radius=Math.max(volume.width,volume.depth)*1.35;

    if(d < .01) {
        const view=lookAt([cx,cy,cz+radius],[cx,cy,cz],[0,1,0]);
        const scale=radius*.62;
        return multiply(
            orthographic(-scale*aspect,scale*aspect,-scale,scale,-radius*3,radius*3),
            view
        );
    }

    let azimuth=Math.PI/4;
    if(behavior==="orbit") azimuth += time*.16;
    if(behavior==="walk") azimuth = time*.055;

    const elevation=.62 + d*.35;
    const followX=behavior==="follow" ? Math.sin(time*.45)*radius*.16 : 0;
    const followZ=behavior==="follow" ? Math.cos(time*.37)*radius*.10 : 0;

    const eye=[
        cx+Math.cos(azimuth)*Math.cos(elevation)*radius+followX,
        cy+Math.sin(elevation)*radius*.82,
        cz+Math.sin(azimuth)*Math.cos(elevation)*radius+followZ
    ];

    const view=lookAt(eye,[cx+followX*.3,cy,cz+followZ*.3],[0,1,0]);
    const orthoScale=radius*.58;
    const ortho=orthographic(-orthoScale*aspect,orthoScale*aspect,-orthoScale,orthoScale,-radius*3,radius*3);
    const persp=perspective(.62,aspect,.1,radius*5);

    // 0 = plan, .5 = isometric, 1 = perspective 3D.
    const t=smooth((d-.5)*2);
    const blended=new Float32Array(16);
    for(let i=0;i<16;i++) blended[i]=ortho[i]+(persp[i]-ortho[i])*t;

    return multiply(blended,view);
}

function rebuildMesh() {
    const mesh=surfaceMesh(lab.volume);
    lab.mesh=mesh;
    lab.vertexCount=mesh.positions.length/3;
    lab.exposed=mesh.exposed;
    lab.voxels=mesh.occupied;

    lab.gl.bindBuffer(lab.gl.ARRAY_BUFFER,lab.positionBuffer);
    lab.gl.bufferData(lab.gl.ARRAY_BUFFER,mesh.positions,lab.gl.DYNAMIC_DRAW);

    lab.gl.bindBuffer(lab.gl.ARRAY_BUFFER,lab.normalBuffer);
    lab.gl.bufferData(lab.gl.ARRAY_BUFFER,mesh.normals,lab.gl.DYNAMIC_DRAW);
}

function makeVolume(name) {
    return name==="massive" ? buildMassive() : buildDetail();
}

export function startVoxelSubstrateLab(canvasId, specimen="detail", dimension=.75, camera="frame") {
    stopVoxelSubstrateLab();

    const canvas=document.getElementById(canvasId);
    if(!canvas) throw new Error("Voxel substrate canvas not found.");

    const gl=canvas.getContext("webgl");
    if(!gl) throw new Error("WebGL is unavailable.");

    const p=program(gl);
    const positionBuffer=gl.createBuffer();
    const normalBuffer=gl.createBuffer();

    lab={
        gl,p,
        positionBuffer,
        normalBuffer,
        position:gl.getAttribLocation(p,"aPosition"),
        normal:gl.getAttribLocation(p,"aNormal"),
        viewProjection:gl.getUniformLocation(p,"uViewProjection"),
        dimensionUniform:gl.getUniformLocation(p,"uDimension"),
        volume:makeVolume(specimen),
        specimen,
        dimension:clamp01(dimension),
        camera,
        vertexCount:0,
        voxels:0,
        exposed:0,
        fps:0,
        frameMs:0,
        frames:0,
        sampleStart:performance.now(),
        raf:0,
        metrics:{fps:0,frameMs:0,voxels:0,exposed:0,vertices:0},
        resizeHandler:null
    };

    const resize=()=>{
        const dpr=Math.min(window.devicePixelRatio||1,2);
        canvas.width=Math.max(1,Math.floor(canvas.clientWidth*dpr));
        canvas.height=Math.max(1,Math.floor(canvas.clientHeight*dpr));
        gl.viewport(0,0,canvas.width,canvas.height);
    };

    lab.resizeHandler=resize;
    window.addEventListener("resize",resize);
    resize();
    rebuildMesh();

    function frame(now) {
        if(!lab) return;
        resize();

        lab.frames++;
        if(now-lab.sampleStart>=500) {
            lab.fps=lab.frames*1000/(now-lab.sampleStart);
            lab.frameMs=1000/(lab.fps||1);
            lab.frames=0;
            lab.sampleStart=now;
            lab.metrics={
                fps:lab.fps,
                frameMs:lab.frameMs,
                voxels:lab.voxels,
                exposed:lab.exposed,
                vertices:lab.vertexCount
            };
        }

        gl.enable(gl.DEPTH_TEST);
        gl.enable(gl.CULL_FACE);
        gl.clearColor(.004,.008,.014,1);
        gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT);
        gl.useProgram(p);

        gl.bindBuffer(gl.ARRAY_BUFFER,positionBuffer);
        gl.enableVertexAttribArray(lab.position);
        gl.vertexAttribPointer(lab.position,3,gl.FLOAT,false,0,0);

        gl.bindBuffer(gl.ARRAY_BUFFER,normalBuffer);
        gl.enableVertexAttribArray(lab.normal);
        gl.vertexAttribPointer(lab.normal,3,gl.FLOAT,false,0,0);

        const aspect=canvas.width/Math.max(1,canvas.height);
        const matrix=cameraMatrix(lab.dimension,lab.camera,now*.001,aspect,lab.volume);

        gl.uniformMatrix4fv(lab.viewProjection,false,matrix);
        gl.uniform1f(lab.dimensionUniform,lab.dimension);
        gl.drawArrays(gl.TRIANGLES,0,lab.vertexCount);

        lab.raf=requestAnimationFrame(frame);
    }

    frame(performance.now());
}

export function setVoxelRenderIntent(dimension,camera) {
    if(!lab) return;
    lab.dimension=clamp01(dimension);
    lab.camera=camera||"frame";
}

export function setVoxelCameraBehavior(camera) {
    if(lab) lab.camera=camera||"frame";
}

export function setVoxelSpecimen(specimen) {
    if(!lab) return;
    lab.specimen=specimen;
    lab.volume=makeVolume(specimen);
    rebuildMesh();
}

export function resetVoxelSpecimen() {
    if(!lab) return;
    lab.volume=makeVolume(lab.specimen);
    rebuildMesh();
}

export function applyVoxelScrapeTool(tool) {
    if(!lab) return;

    const v=lab.volume;
    const cx=v.width/2;
    const cy=v.height/2;
    const cz=v.depth/2;

    if(tool==="sphere") {
        clearSphere(v,cx,cy,cz,Math.min(v.width,v.depth)*.18);
    } else if(tool==="tunnel") {
        carveLine(
            v,
            [v.width*.12,v.height*.42,v.depth*.15],
            [v.width*.88,v.height*.68,v.depth*.85],
            Math.max(2.2,Math.min(v.width,v.depth)*.045)
        );
    } else if(tool==="slice") {
        const x0=Math.floor(v.width*.42);
        const x1=Math.floor(v.width*.58);
        for(let z=0;z<v.depth;z++)
            for(let y=0;y<v.height;y++)
                for(let x=x0;x<x1;x++)
                    v.cells[index(v,x,y,z)]=0;
    } else if(tool==="box") {
        clearSphere(v,cx,cy,cz,Math.min(v.width,v.depth)*.28);
        fillBox(
            v,
            Math.floor(v.width*.35),
            Math.floor(v.height*.28),
            Math.floor(v.depth*.35),
            Math.floor(v.width*.65),
            Math.floor(v.height*.72),
            Math.floor(v.depth*.65)
        );
    }

    rebuildMesh();
}

export function getVoxelMetrics() {
    if(!lab)
        return {fps:0,frameMs:0,voxels:0,exposed:0,vertices:0};
    return lab.metrics;
}

export function stopVoxelSubstrateLab() {
    if(!lab) return;
    cancelAnimationFrame(lab.raf);
    if(lab.resizeHandler)
        window.removeEventListener("resize",lab.resizeHandler);

    lab.gl.deleteBuffer(lab.positionBuffer);
    lab.gl.deleteBuffer(lab.normalBuffer);
    lab.gl.deleteProgram(lab.p);
    lab=null;
}
