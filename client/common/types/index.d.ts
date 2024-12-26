export type Albums = {
    id: string,
    name: string,
    genre: string,
    year: number,
    artist: Artists
}

export type Artists = {
    id: string,
    name: string
}

export type Artist = {
    name: string,
    genre: string,
    bio: string
}

export type Tracks = {
    id: string,
    name: string,
    duration: number,
    number: number,
    isStarred: boolean,
    artistIds: string[],
    albumId: string
}

export type Album = {
    name: string,
    genre: string,
    year: number,
    artist: Artist,
    id: string
}

export type Track = {
    name: string,
    duration: number,
    number: number,
    album: Album,
    artists: Artist[],
    id: string
}

export type Playlist = {
    name: string,
    tracks: Tracks[],
    id: string
}