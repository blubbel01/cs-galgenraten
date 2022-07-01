import {ItemFlags} from "./ItemFlags";

export interface ItemMeta {
    readonly displayName: string;
    readonly lore: string;
    readonly flagList: ItemFlags[];
    readonly attributeModifiers: { [key: string]: number };
}