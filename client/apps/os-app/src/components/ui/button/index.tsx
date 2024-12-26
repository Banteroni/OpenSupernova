import { ReactNode, useState } from "react"
import ButtonComponent from "./ButtonComponent"

export type ButtonProps = {
    icon?: ReactNode
    onClick: () => void
    children?: ReactNode
    text?: string
    disabled?: boolean
}

export default function Button(props: ButtonProps) {

    const [isHovered, setIsHovered] = useState(false)

    return ButtonComponent({ ...props, isHovered, setIsHovered })
}