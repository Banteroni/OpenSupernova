type Albums = {
    id: string
    name: string,
    genre: string,
    year: number,
    artist: {
        id: string,
        name: string
    }
}