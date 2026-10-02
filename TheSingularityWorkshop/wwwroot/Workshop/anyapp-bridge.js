window.workshopAnyApp = (() => {
    const bridgePort = 47631;
    const protocol = 1;
    let socket = null;
    let session = null;
    let reconnectTimer = 0;

    function createToken() {
        if (globalThis.crypto?.randomUUID) {
            return globalThis.crypto.randomUUID();
        }

        const bytes = new Uint8Array(24);
        crypto.getRandomValues(bytes);
        return Array.from(bytes, value => value.toString(16).padStart(2, "0")).join("");
    }

    async function getCapabilities() {
        const modes = [];
        if (navigator.xr?.isSessionSupported) {
            for (const mode of ["inline", "immersive-vr", "immersive-ar"]) {
                try {
                    if (await navigator.xr.isSessionSupported(mode)) {
                        modes.push(mode);
                    }
                } catch {
                    // A browser may expose WebXR while denying a particular mode.
                }
            }
        }

        return {
            origin: window.location.origin,
            userAgent: navigator.userAgent,
            platform: navigator.platform,
            userAgentData: navigator.userAgentData
                ? {
                    mobile: navigator.userAgentData.mobile,
                    platform: navigator.userAgentData.platform
                }
                : null,
            viewport: {
                width: window.innerWidth,
                height: window.innerHeight,
                devicePixelRatio: window.devicePixelRatio
            },
            visibility: document.visibilityState,
            focused: document.hasFocus(),
            webxr: Boolean(navigator.xr),
            xrSessionModes: modes
        };
    }

    function send(type, payload = {}) {
        if (!socket || socket.readyState !== WebSocket.OPEN) {
            return false;
        }

        socket.send(JSON.stringify({
            protocol,
            type,
            sessionId: session?.sessionId ?? null,
            sequence: Date.now(),
            timestampUtc: new Date().toISOString(),
            payload
        }));

        return true;
    }

    async function connect(token, onMessage) {
        if (!token) {
            throw new Error("AnyApp bridge launch token is required.");
        }

        if (socket) {
            socket.close();
        }

        const url = `ws://127.0.0.1:${bridgePort}/bridge?token=${encodeURIComponent(token)}`;
        socket = new WebSocket(url);

        socket.addEventListener("open", async () => {
            const capabilities = await getCapabilities();
            send("hello", { capabilities });
        });

        socket.addEventListener("message", event => {
            let message;
            try {
                message = JSON.parse(event.data);
            } catch {
                return;
            }

            if (message.type === "welcome") {
                session = {
                    sessionId: message.payload?.sessionId ?? null,
                    connectedUtc: new Date().toISOString()
                };
            }

            onMessage?.(message);
        });

        socket.addEventListener("close", () => {
            session = null;
        });

        return socket;
    }

    function launchExperience(experienceId, version, contentHash, options = {}) {
        const token = options.token ?? createToken();
        const uri =
            `anyapp://experience/${encodeURIComponent(experienceId)}/` +
            `${encodeURIComponent(version)}/${encodeURIComponent(contentHash)}` +
            `?token=${encodeURIComponent(token)}`;

        const anchor = document.createElement("a");
        anchor.href = uri;
        anchor.rel = "noreferrer";
        anchor.style.display = "none";
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();

        window.setTimeout(() => {
            connect(token, options.onMessage).catch(() => {
                // AnyApp may take a moment to start. Retry without surfacing
                // transport noise to the Experience itself.
                let attempts = 0;
                const retry = () => {
                    if (++attempts > 20) {
                        return;
                    }

                    connect(token, options.onMessage)
                        .catch(() => window.setTimeout(retry, 500));
                };

                retry();
            });
        }, options.connectDelayMs ?? 750);

        return {
            token,
            uri,
            connect: () => connect(token, options.onMessage)
        };
    }

    function sendEvent(name, data = {}) {
        return send("event", { name, data });
    }

    function heartbeat() {
        return send("heartbeat", {});
    }

    return {
        connect,
        launchExperience,
        sendEvent,
        heartbeat,
        getCapabilities
    };
})();
