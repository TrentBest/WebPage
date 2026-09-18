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
        uniform float uDepth;
        void main() {
            vec3 p = aPosition;
            p.z *= uDepth;
            gl_Position = uViewProjection * vec4(p, 1.0);
        }
    `);

    const fragment = makeShader(gl, gl.FRAGMENT_SHADER, `
        precision mediump float;
        uniform float uDepth;
        void main() {
            vec3 flatColor = vec3(0.12, 0.72, 1.0);
            vec3 volumeColor = vec3(1.0, 0.32, 0.84);
            gl_FragColor = vec4(mix(flatColor, volumeColor, uDepth), 0.92);
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
                a[r] * b[c * 4] +
                a[4 + r] * b[c * 4 + 1] +
                a[8 + r] * b[c * 4 + 2] +
                a[12 + r] * b[c * 4 + 3];
    return out;
}

function sub(a, b) { return [a[0] - b[0], a[1] - b[1], a[2] - b[2]]; }
function dot(a, b) { return a[0] * b[0] + a[1] * b[1] + a[2] * b[2]; }
function cross(a, b) { return [a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0]]; }
function normalize(a) {
    const n = Math.hypot(a[0], a[1], a[2]) || 1;
    return [a[0] / n, a[1] / n, a[2] / n];
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

function lerp(a, b, t) {
    return a + (b - a) * t;
}

function smoothstep(t) {
    const x = Math.max(0, Math.min(1, t));
    return x * x * (3 - 2 * x);
}

function dimensionLines() {
    const vertices = [];
    const add = (a, b) => vertices.push(a[0], a[1], a[2], b[0], b[1], b[2]);

    // A deliberately simple "research object": one ship, authored once in 3D.
    // At depth 0 every Z coordinate collapses into the same 2D drawing plane.
    add([-55, 0, 0], [55, 0, 0]);
    add([-45, -12, 0], [45, -12, 0]);
    add([-45, 12, 0], [45, 12, 0]);
    add([-55, 0, 0], [-45, -12, 0]);
    add([-55, 0, 0], [-45, 12, 0]);
    add([55, 0, 0], [45, -12, 0]);
    add([55, 0, 0], [45, 12, 0]);

    for (let i = 0; i < 13; i++) {
        const x = -45 + i * (90 / 12);
        const half = 12 - Math.abs(x) * 0.055;
        const z = 3 + Math.sin(i * 0.9) * 2.5;
        add([x, -half, -z], [x, half, z]);
        add([x, -half, z], [x, half, -z]);
        add([x, -half, -z], [x, -half, z]);
        add([x, half, -z], [x, half, z]);
    }

    // Bridge and service spine.
    add([-22, 18, -8], [22, 18, 8]);
    add([-22, -18, 8], [22, -18, -8]);
    add([-18, -18, 8], [-18, 18, -8]);
    add([18, -18, -8], [18, 18, 8]);

    // A few dimensional guide planes make the emergence of Z legible.
    for (const x of [-36, -18, 0, 18, 36]) {
        add([x, -16, -14], [x, 16, 14]);
        add([x, -16, 14], [x, 16, -14]);
    }

    return new Float32Array(vertices);
}

export function startDimensionalResearchLab(canvasId) {
    stopDimensionalResearchLab();

    const canvas = document.getElementById(canvasId);
    if (!canvas) throw new Error("Dimensional research canvas not found.");

    const gl = canvas.getContext("webgl") || canvas.getContext("experimental-webgl");
    if (!gl) throw new Error("WebGL is unavailable in this browser.");

    const program = makeProgram(gl);
    const buffer = gl.createBuffer();
    const position = gl.getAttribLocation(program, "aPosition");
    const viewProjection = gl.getUniformLocation(program, "uViewProjection");
    const depthUniform = gl.getUniformLocation(program, "uDepth");
    const data = dimensionLines();

    lab = {
        gl,
        program,
        buffer,
        position,
        viewProjection,
        depthUniform,
        vertexCount: data.length / 3,
        currentDepth: 0,
        targetDepth: 0,
        transitionStart: 0,
        transitionFrom: 0,
        transitionTo: 0,
        transitionDuration: 850,
        transitionActive: false,
        raf: 0,
        fps: 0,
        frameMs: 0,
        frames: 0,
        fpsStart: performance.now()
    };

    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, data, gl.STATIC_DRAW);

    function resize() {
        const dpr = Math.min(window.devicePixelRatio || 1, 2);
        const width = Math.max(1, Math.floor(canvas.clientWidth * dpr));
        const height = Math.max(1, Math.floor(canvas.clientHeight * dpr));
        if (canvas.width !== width || canvas.height !== height) {
            canvas.width = width;
            canvas.height = height;
        }
        gl.viewport(0, 0, width, height);
    }

    lab.setDimension = (enabled) => {
        const target = enabled ? 1 : 0;
        if (target === lab.targetDepth && lab.transitionActive) return;
        lab.transitionFrom = lab.currentDepth;
        lab.transitionTo = target;
        lab.targetDepth = target;
        lab.transitionStart = performance.now();
        lab.transitionActive = true;
    };

    function frame(now) {
        if (!lab) return;

        resize();

        if (lab.transitionActive) {
            const t = Math.min(1, (now - lab.transitionStart) / lab.transitionDuration);
            const eased = smoothstep(t);
            lab.currentDepth = lerp(lab.transitionFrom, lab.transitionTo, eased);
            if (t >= 1) lab.transitionActive = false;
        }

        lab.frames++;
        if (now - lab.fpsStart >= 500) {
            lab.fps = lab.frames * 1000 / (now - lab.fpsStart);
            lab.frameMs = 1000 / (lab.fps || 1);
            lab.frames = 0;
            lab.fpsStart = now;
        }

        const aspect = canvas.width / Math.max(1, canvas.height);
        const projection = perspective(Math.PI / 3, aspect, 0.1, 500);

        const eye = [
            lerp(0, 105, lab.currentDepth),
            lerp(0, 68, lab.currentDepth),
            lerp(165, 110, lab.currentDepth)
        ];

        const view = lookAt(eye, [0, 0, 0], [0, 1, 0]);

        gl.enable(gl.DEPTH_TEST);
        gl.clearColor(0.004, 0.008, 0.018, 1);
        gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
        gl.useProgram(program);
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        gl.enableVertexAttribArray(position);
        gl.vertexAttribPointer(position, 3, gl.FLOAT, false, 0, 0);
        gl.uniformMatrix4fv(viewProjection, false, multiply(projection, view));
        gl.uniform1f(depthUniform, lab.currentDepth);
        gl.drawArrays(gl.LINES, 0, lab.vertexCount);

        lab.raf = requestAnimationFrame(frame);
    }

    window.addEventListener("resize", resize);
    frame(performance.now());
}

export function setDimensionalState(enabled) {
    if (lab?.setDimension) lab.setDimension(!!enabled);
}

export function getDimensionalResearchMetrics() {
    if (!lab) return { depth: 0, targetDepth: 0, fps: 0, frameMs: 0 };
    return {
        depth: lab.currentDepth,
        targetDepth: lab.targetDepth,
        fps: lab.fps,
        frameMs: lab.frameMs
    };
}

export function stopDimensionalResearchLab() {
    if (!lab) return;
    cancelAnimationFrame(lab.raf);
    window.removeEventListener("resize", () => {});
    lab.gl.deleteBuffer(lab.buffer);
    lab.gl.deleteProgram(lab.program);
    lab = null;
}
