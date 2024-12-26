import { ReactNode } from "react";
import MenuRouteComponent from "./MenuRouteComponent";
import { useLocation, useNavigate } from "react-router";

export type MenuRouteProps = {
    text: string
    new?: boolean
    icon?: ReactNode
    url: string
}

export default function MenuRoute(props: MenuRouteProps) {
    const navigate = useNavigate()
    const location = useLocation()
    const active = location.pathname == props.url

    const routeTo = () => {
        navigate(props.url)
    }
    return <MenuRouteComponent inherited={props} onClick={routeTo} active={active} />
}