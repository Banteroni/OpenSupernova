import { ReactNode } from "react"

export type TextFieldComponentProps = {
    placeholder: string
    icon?: ReactNode,
    type?: string,
    error?: string,
    name?: string,
}

export default function ControlledTextFieldComponent(props: TextFieldComponentProps) {
    return (<div><div className=" bg-white/5 text-white rounded-md  flex gap-x-3 w-full p-3">
        {props.icon && (
            <div className="flex items-center text-primary">
                {props.icon}
            </div>
        )}
        <input
            className="w-full bg-transparent text-white placeholder:opacity-50 border-none outline-none"
            placeholder={props.placeholder}
            name={props.name}
            type={props.type || "text"}
        />
    </div>{props.error && <div className="text-primary">{props.error}</div>}</div>)
}   