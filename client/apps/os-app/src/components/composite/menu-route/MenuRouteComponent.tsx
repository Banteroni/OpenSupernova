import { MenuRouteProps } from ".";

export default function MenuRouteComponent(props: {
    inherited: MenuRouteProps
    onClick: () => void,
    active: boolean
}) {
    return <div className="p-2 hover:bg-white/5 rounded duration-200 cursor-pointer flex gap-x-3 items-center text-sm" onClick={props.onClick}>
        {props.inherited.icon && <span className={`${props.active ? "text-primary" : "text-white"}`}>{props.inherited.icon}</span>}
        <span >{props.inherited.text}</span>
        {props.inherited.new && <div className="flex-1 flex justify-end ">
            <div className="h-1 w-1 bg-primary rounded-full" />
        </div>}
    </div>
}