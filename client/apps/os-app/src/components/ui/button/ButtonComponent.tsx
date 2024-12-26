import { ReactNode } from "react"

export type ButtonComponentProps = {
    icon?: ReactNode
    onClick: () => void
    children?: ReactNode
    text?: string
    isHovered: boolean
    disabled?: boolean
    setIsHovered: (isHovered: boolean) => void
}

export default function ButtonComponent(props: ButtonComponentProps) {
    return (
        <button className={`py-3 px-5 bg-primary rounded-md text-white relative ${props.disabled && "opacity-60"}`} onClick={props.onClick} onMouseEnter={() => props.setIsHovered(true)} onMouseLeave={() => props.setIsHovered(false)}>
            {props.isHovered && <div className="absolute inset-0 bg-black/20 rounded-md pointer-events-none" />}
            {props.icon && (
                <div className="flex items-center text-white">
                    {props.icon}
                </div>
            )}
            {props.text || props.children}
        </button>
    )

}