import { Albums, Playlist } from "opensupernova";
import Album from "../../components/composite/album";
import PlaylistComponent from "../../components/composite/playlist";

export default function HomeComponent(props: {
    albums: Albums[] | null,
    playlistData: Playlist[] | null,
    loading: boolean
}) {

    const albumsData = props.albums
    const playlistsData = props.playlistData
    return <div className="flex flex-col gap-y-20">
        <div className="flex flex-col gap-y-6">
            <h1>Playlists</h1>
            <div className="flex gap-x-6 w-full">
                <div className="bg-primary rounded-xl w-96 relative p-8 justify-end flex-col flex">
                    <div className=" bg-gradient-to-br from-black/0 to to-black/20  absolute top-0 bottom-0 left-0 right-0" />
                    <div className="z-10">
                        <h1 className="text-2xl">Starred songs</h1>
                        <p className="text-sm">You have starred 120 songs</p>
                    </div>
                </div>
                {playlistsData && playlistsData.map((playlist) => <PlaylistComponent key={playlist.id} imageUrl={"https://cdn-images.dzcdn.net/images/cover/c41dd72d962458d2187132f55a3ca711/200x200.jpg"} title={playlist.name} id={playlist.id} songs={playlistsData.length} />)}
            </div>
        </div>
        <div className="flex flex-col gap-y-6">
            <h1>Albums</h1>
            <div className="flex gap-x-6 w-full">
                {albumsData && albumsData.map((album) => <Album key={album.id} imageUrl={"https://cdn-images.dzcdn.net/images/cover/c41dd72d962458d2187132f55a3ca711/200x200.jpg"} title={album.name} id={album.id} songs={albumsData.length} />)}
            </div>
        </div>
    </div>
}