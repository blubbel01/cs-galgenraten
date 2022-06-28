import {ItemFlags} from "./ItemFlags";

export interface ItemMeta {
    readonly displayName: string;
    readonly lore: string;
    readonly damage: number;
    readonly flagList: ItemFlags[];
    readonly attributeModifiers: { [key: string]: number };
}