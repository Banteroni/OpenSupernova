import React, { ReactNode } from "react"

export type ControlledTextFieldComponentProps = {
    placeholder: string
    value: string
    onChange: (e: React.ChangeEvent<any>) => void
    icon?: ReactNode,
    type?: string,
    error?: string,
    name?: string,
}

export default function ControlledTextFieldComponent(props: ControlledTextFieldComponentProps) {
    return (

        <div><div className=" bg-white/5 text-white rounded-md p-3 flex gap-x-3 w-full">
            {props.icon && (
                <div className="flex items-center text-primary">
                    {props.icon}
                </div>
            )}
            <input
                className="w-full bg-transparent text-white placeholder:opacity-50 border-none outline-none"
                placeholder={props.placeholder}
                value={props.value}
                type={props.type || "text"}
                name={props.name}
                onChange={props.onChange}
            />
        </div>
            {props.error && <div className="text-primary">{props.error}</div>}</div>)
}   