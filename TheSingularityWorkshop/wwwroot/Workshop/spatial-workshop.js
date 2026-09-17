/// <summary>Browser-side coordinate bridge for the avatar-centered Workshop camera.</summary>
export function screenToWorld(clientX, clientY, avatarX, avatarY) {
    const width = Math.max(window.innerWidth, 1);
    const height = Math.max(window.innerHeight, 1);
    const screenVw = clientX / width * 100;
    const screenVh = clientY / height * 100;
    const offsetVw = 50 - avatarX * 1.5;
    const offsetVh = 50 - avatarY * 1.5;
    return {
        x: (screenVw - offsetVw) / 1.5,
        y: (screenVh - offsetVh) / 1.5
    };
}

/// <summary>Converts a click into world coordinates for a 66.666-unit centered camera viewBox.</summary>
export function screenToCenteredWorld(clientX, clientY, avatarX, avatarY) {
    const width = Math.max(window.innerWidth, 1);
    const height = Math.max(window.innerHeight, 1);
    const screenVw = clientX / width * 100;
    const screenVh = clientY / height * 100;
    const halfView = 33.333;
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
