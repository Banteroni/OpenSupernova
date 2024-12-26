import Album from "../../components/composite/album";
import Playlist from "../../components/composite/playlist";
import useFetch from "../../hooks/useFetch";

export default function HomeComponent() {
    const [data, error, loading] = useFetch<Albums[]>("/api/albums")

    console.log(data, error, loading)
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
                {data && data.map((album) => <Playlist key={album.id} imageUrl={"https://cdn-images.dzcdn.net/images/cover/c41dd72d962458d2187132f55a3ca711/200x200.jpg"} title={album.name} id={album.id} songs={data.length} />)}
            </div>
        </div>
        <div className="flex flex-col gap-y-6">
            <h1>Albums</h1>
            <div className="flex gap-x-6 w-full">
                {data && data.map((album) => <Album key={album.id} imageUrl={"https://cdn-images.dzcdn.net/images/cover/c41dd72d962458d2187132f55a3ca711/200x200.jpg"} title={album.name} id={album.id} songs={data.length} />)}
            </div>
        </div>
    </div>
}