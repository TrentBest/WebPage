/// <summary>Browser-side coordinate bridge for the avatar-centered Workshop camera.</summary>
export function screenToWorld(clientX, clientY, avatarX, avatarY, zoom = 1) {
    const width = Math.max(window.innerWidth, 1);
    const height = Math.max(window.innerHeight, 1);
    const screenVw = clientX / width * 100;
    const screenVh = clientY / height * 100;
    const safeZoom = Math.max(0.35, Math.min(4, zoom));
    const offsetVw = 50 - avatarX * 1.5 * safeZoom;
    const offsetVh = 50 - avatarY * 1.5 * safeZoom;
    return {
        x: (screenVw - offsetVw) / (1.5 * safeZoom),
        y: (screenVh - offsetVh) / (1.5 * safeZoom)
    };
}

/// <summary>Converts a click into world coordinates for the centered camera used by line-rendered interiors.</summary>
export function screenToCenteredWorld(clientX, clientY, avatarX, avatarY, zoom = 1) {
    const width = Math.max(window.innerWidth, 1);
    const height = Math.max(window.innerHeight, 1);
    const screenVw = clientX / width * 100;
    const screenVh = clientY / height * 100;
    const safeZoom = Math.max(0.35, Math.min(4, zoom));
    const halfView = 33.333 / safeZoom;
    const unitsPerScreen = (halfView * 2) / 100;
    return {
        x: avatarX + (screenVw - 50) * unitsPerScreen,
        y: avatarY + (screenVh - 50) * unitsPerScreen
    };
}

export function screenToLinePoint(clientX, clientY) {
    const width = Math.max(window.innerWidth, 1);
    const height = Math.max(window.innerHeight, 1);
    return {
        x: clientX / width * 100,
        y: clientY / height * 100
    };
}
