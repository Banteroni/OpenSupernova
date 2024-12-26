import { createSlice } from "@reduxjs/toolkit";

export const playerSlice = createSlice({
    name: "player",
    initialState: {
        isPlaying: false,
        albumUrl: "",
    },
    reducers: {
        setPlay: (state, payload) => {
            if (payload.type === "boolean") {
                state.isPlaying = payload.payload
            }
        },
        setAlbumUrl: (state, action) => {
            if (action.type === "string") {
                state.albumUrl = action.payload
            }
        }
    }
})

export const { setPlay, setAlbumUrl } = playerSlice.actions;

export default playerSlice.reducer