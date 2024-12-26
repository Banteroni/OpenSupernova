import { Ethernet, Key, Person } from "react-bootstrap-icons";
import LogoTransparent from "../../assets/logo_transparent.svg";
import TextField from "../../components/ui/text-field";
import Button from "../../components/ui/button";
import { Formik } from "formik";

export type LoginComponentProps = {
    validateLogin(values: {
        [key: string]: string;
    }): { [key: string]: string; }
    submitLogin(values: {
        [key: string]: string;
    }): Promise<any>
}

export default function LoginComponent(props: LoginComponentProps) {
    return (
        <main className="bg-background h-screen w-screen flex flex-col items-center">
            <div className="flex flex-col items-center flex-1 justify-end">
                <div className="w-[120px] relative aspect-square  animate-slightYMove ">
                    <img src={LogoTransparent} alt="Logo" className="mx-auto absolute top-0 left-0 right-0" />
                    <img src={LogoTransparent} alt="Logo" className="mx-auto opacity-70 absolute top-3 left-0 right-0" />
                    <img src={LogoTransparent} alt="Logo" className="mx-auto opacity-30 absolute top-6 left-0 right-0" />
                </div>
                <h1 className="pt-20">OpenSupernova</h1>
                <p className="subtitle">Your app, your music, your experience</p>
            </div>
            <Formik
                initialValues={{ serverUrl: "", username: "", password: "" }}
                onSubmit={props.submitLogin}
                validateOnChange={false}
                validate={props.validateLogin}
            >
                {({
                    values,
                    errors,
                    handleChange,
                    handleSubmit,
                    isSubmitting,
                }) => (
                    <form className="w-[300px] flex flex-col gap-y-5 flex-1 justify-center pt-5">
                        <TextField icon={<Ethernet />} value={values.serverUrl} onChange={handleChange} error={errors.serverUrl} name="serverUrl" placeholder="Server URL" />
                        <TextField icon={<Person />} value={values.username} onChange={handleChange} error={errors.username} placeholder="Username" name="username" />
                        <TextField icon={<Key />} value={values.password} onChange={handleChange} error={errors.password} placeholder="Password" name="password" type="password" />
                        <div className="flex pt-5" />
                        <Button onClick={handleSubmit} disabled={isSubmitting} text="Login" />
                    </form>
                )}

            </Formik>
            <div className=" flex-1" />
        </main >
    )
}