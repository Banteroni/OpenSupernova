import { ReactNode } from "react"
import ControlledTextFieldComponent from "./ControlledTextFieldComponent"
import TextFieldComponent from "./TextFieldComponent"

export type TextFieldProps = {
    placeholder: string
    icon?: ReactNode,
    type?: string,
    value?: string,
    name?: string,
    error?: string,
    onChange?: (value: React.ChangeEvent<any>) => any
}

export default function TextField(props: TextFieldProps) {
    if (props.onChange) {
        return ControlledTextFieldComponent({
            ...props,
            onChange: props.onChange,
            value: props.value || ""
        })
    }
    return TextFieldComponent(props)
}