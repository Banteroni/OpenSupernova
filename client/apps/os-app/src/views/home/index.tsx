import { useEffect } from "react";
import useFetch from "../../hooks/useFetch";
import HomeComponent from "./HomeComponent";
import { useNavigate } from "react-router";
import { Albums, Playlist } from "opensupernova";

export default function Home() {
    const navigate = useNavigate()
    const [albumsData, albumsError, albumsLoading, albumsFetch] = useFetch<Albums[]>("/api/albums")
    const [playlistsData, playlistsError, playlistsLoading, playlistFetch] = useFetch<Playlist[]>("/api/playlists")

    useEffect(() => {
        if (albumsError || playlistsError) {
            navigate("/login")
        }
    }, [albumsError, playlistsError])

    const loading = !!albumsLoading || !!playlistsLoading

    return <HomeComponent albums={albumsData as Albums[]} playlistData={playlistsData as Playlist[]} loading={loading} />
}