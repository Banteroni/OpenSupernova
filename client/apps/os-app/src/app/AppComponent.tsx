import { Outlet } from "react-router";
import "./index.css"
import Logo from "../assets/logo.svg";
import MenuRoute from "../components/composite/menu-route";
import { Columns, House, People, Vinyl } from "react-bootstrap-icons";
import SideBarPlayer from "../components/composite/sidebar-player";

export default function AppComponent() {
    return <main className="grid grid-cols-6 gap-x-5 h-screen w-screen">
        <nav className="col-span-1 bg-black flex flex-col">
            <div>
                <img src={Logo} alt="Logo" className="h-8 m-8" />
            </div>
            <div className="p-8">
                <div className="section">
                    <span className="menu-voice">Welcome back, John</span>
                    <MenuRoute text="Home" url="/app" new icon={<House />} />
                    <MenuRoute text="Albums" url="/albums" icon={<Vinyl />} />
                    <MenuRoute text="Artists" url="/artists" icon={<People />} />
                    <MenuRoute text="Playlists" url="/playlists" icon={<Columns />} />
                </div>
            </div>
            <div className="p-8">
                <div className="section">
                    <span className="menu-voice">Playlists</span>
                    <MenuRoute text="Rediscovering old gems" url="/app" new />
                    <MenuRoute text="My Amazing playlist" url="/app" />
                    <MenuRoute text="Discover Weekly" url="/app" />
                </div>
            </div>
            <div className="p-8">
                <div className="section">
                    <span className="menu-voice">Latest searches</span>
                    <MenuRoute text="Tranquility Base Hotel & Casino" url="/app" />
                    <MenuRoute text="Oasis" url="/app" />
                    <MenuRoute text="The Miracle" url="/app" />
                </div>
            </div>
            <SideBarPlayer />
        </nav>
        <div className="py-20 px-12 w-full col-span-5">
            <Outlet />
        </div>
    </main>
}