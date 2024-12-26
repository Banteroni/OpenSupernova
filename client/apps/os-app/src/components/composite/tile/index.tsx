import TileComponent from "./TileComponent"

export type TileProps = {
    id: string
    title: string,
    imageUrl: string,
    imageOnClick?: () => any
    roundImage?: boolean
    titleOnClick?: () => any
    subtitle?: string
    subtitleTrigger?: () => any
    subtitleOnClick?: () => any

}

export default function Tile(props: TileProps) {
    return <TileComponent  {...props} />
}