const AudioContextType = window.AudioContext || window.webkitAudioContext;
let context;

function getContext() {
    context ??= new AudioContextType();
    if (context.state === "suspended") context.resume();
    return context;
}

export function play(kind) {
    if (!AudioContextType) return;
    const audio = getContext();
    const oscillator = audio.createOscillator();
    const gain = audio.createGain();
    const now = audio.currentTime;
    const settings = { wall: [420, 0.045], paddle: [720, 0.065], score: [180, 0.22] }[kind] ?? [500, 0.05];
    oscillator.type = kind === "score" ? "sawtooth" : "square";
    oscillator.frequency.setValueAtTime(settings[0], now);
    oscillator.frequency.exponentialRampToValueAtTime(settings[0] * 0.65, now + settings[1]);
    gain.gain.setValueAtTime(0.0001, now);
    gain.gain.exponentialRampToValueAtTime(kind === "score" ? 0.12 : 0.055, now + 0.004);
    gain.gain.exponentialRampToValueAtTime(0.0001, now + settings[1]);
    oscillator.connect(gain).connect(audio.destination);
    oscillator.start(now);
    oscillator.stop(now + settings[1] + 0.01);
}
