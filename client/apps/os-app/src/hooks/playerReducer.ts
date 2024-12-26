import { createSlice } from "@reduxjs/toolkit";


export type PlayerState = {
    isPlaying: boolean,
    albumUrl: string,
    artist: {
        name: string,
        id: string,
    },
    album: {
        name: string,
        id: string,
    },
    track: {
        name: string,
        id: string,
    }
}

export const playerSlice = createSlice({
    name: "player",
    initialState: {
        isPlaying: false,
        artist: {
            name: "",
            id: "",
        },
        album: {
            name: "",
            id: "",
            url: "",
        },
        track: {
            name: "",
            id: "",
        }
    },
    reducers: {
        setPlay: (state, payload) => {
            if (payload.type === "boolean") {
                state.isPlaying = payload.payload
            }
        },
        setArtist: (state, action) => {
            state.artist = action.payload
        },
        setAlbum: (state, action) => {
            state.album = action.payload
        },
        setTrack: (state, action) => {
            state.track = action.payload
        }

    }
})

export const { setPlay, setAlbum, setArtist, setTrack } = playerSlice.actions;

export default playerSlice.reducer