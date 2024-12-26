export default class Player {
    // audio properties
    private _audioContext: AudioContext = new AudioContext();
    private _decodedAudio: AudioBuffer | null = null;
    private _gainNode: GainNode = this._audioContext.createGain();

    // props for tracking audio
    private _startTime = 0;
    private _byteOffset = 0;

    // Properties
    public Volume = 0.5;
    public CurrentTrackLength = 0;
    public TrackDuration = 0;
    public TotalFileSize = 0;
    public ChunkSize = 1024 * 1024;

    // Endpoint configuration
    private _baseUrl: string;
    private _token: string;

    constructor(baseUrl: string, token: string) {
        this._baseUrl = baseUrl;
        this._token = token;
        this.createAudioContext();
    }

    public Stream = async (track: string): Promise<void> => {
        var buffer = new Uint8Array() as Uint8Array;
        for (let i = 0; this._byteOffset < this.TotalFileSize || this.TotalFileSize == 0; i++) {
            buffer = await this.fetchChunk(buffer, track);
            var newBuffer = new Uint8Array(buffer);
            var newDecodedAudio = await this._audioContext.decodeAudioData(newBuffer.buffer);
            if (i == 0) {
                this._startTime = this._audioContext.currentTime;
            }
            if (this._audioContext.state === 'running') {
                this._setCurrentTrackLength(this._decodedAudio?.duration || 0)
                var source = this._audioContext.createBufferSource();
                source.buffer = newDecodedAudio;
                source.connect(this._gainNode);  // Connect to GainNode
                source.start(this._startTime + this.CurrentTrackLength, this.CurrentTrackLength);
            }
            this._decodedAudio = newDecodedAudio;
            this._setCurrentTrackLength(this._decodedAudio.duration);
        }
    };

    public Play = () => {
        this._Play();
    }

    public Pause = () => {
        this.Pause();
    }

    protected _Play = async () => {
        if (this._audioContext.state === 'running') {
            return;
        }
        var source = this._audioContext.createBufferSource();
        source.buffer = this._decodedAudio;
        source.connect(this._gainNode);  // Connect to GainNode
        source.start(0);
        this._audioContext.resume();
    }

    protected _Pause = () => {
        if (this._audioContext.state === 'running') {
            this._audioContext.suspend();
        }
    }

    public setVolume = async (value: number) => {
        this._setVolume(value);
    }

    public setTime = async (value: number) => {
        this._setTime(value);
    }

    protected _setCurrentTrackLength = async (value: number) => {
        this.CurrentTrackLength = value;
    }

    protected _setVolume(value: number) {
        this.Volume = value / 100;
        if (this._gainNode) {
            this._gainNode.gain.setValueAtTime(this.Volume, this._audioContext.currentTime);
        }
    }

    protected _setTime = async (value: number) => {
        var desiredTime = (value / 100) * this.TrackDuration;
        this.resetAudioContext();
        var source = this._audioContext.createBufferSource();
        source.buffer = this._decodedAudio;
        source.connect(this._gainNode);  // Connect to GainNode
        source.start(0, desiredTime, this._decodedAudio?.duration);
        this._audioContext.resume();
    }

    protected _setTrackDuration = async (value: number) => {
        this.TrackDuration = value;
    }

    protected _setTotalFileSize = async (value: number) => {
        this.TotalFileSize = value;
    }

    private resetAudioContext() {
        this._audioContext.close();
        this.createAudioContext();
    }

    private createAudioContext() {
        this._audioContext = new (window.AudioContext)();

        this._gainNode = this._audioContext.createGain();  // Create GainNode
        this._gainNode.gain.setValueAtTime(this.Volume, this._audioContext.currentTime); // Set initial volume
        this._gainNode.connect(this._audioContext.destination);  // Connect GainNode to destination

        this._audioContext.suspend();
    }

    private fetchChunk = async (buffer: Uint8Array, track: string) => {
        var effectiveChunkSize = this.ChunkSize - 1;
        if (this.TotalFileSize > 0 && (this._byteOffset + effectiveChunkSize) > this.TotalFileSize) {
            effectiveChunkSize = this.TotalFileSize - this._byteOffset - 1;
        }

        const response = await fetch(this._baseUrl + "/tracks/" + track + "/stream", {
            headers: {
                'Range': `bytes=${this._byteOffset}-${this._byteOffset + effectiveChunkSize}`,
                'Authorization': `Bearer ${this._token}`
            }
        });
        const contentRange = response.headers.get('Content-Range');
        const trackDuration = response.headers.get('X-Track-Duration');
        if (this.TrackDuration == 0 && trackDuration) {
            this._setTrackDuration(parseInt(trackDuration));
        }
        this._setTotalFileSize(contentRange ? parseInt(contentRange.split('/')[1]) : 0);
        var newBuffer = new Uint8Array(await response.arrayBuffer());
        var oldBuffer = new Uint8Array(buffer);
        buffer = new Uint8Array(oldBuffer.length + newBuffer.length);
        buffer.set(oldBuffer);
        buffer.set(newBuffer, oldBuffer.length);
        this._byteOffset += this.ChunkSize;
        return buffer;
    };
}
