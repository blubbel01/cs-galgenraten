import {ItemMeta} from "./ItemMeta";

export interface ItemStack {
    readonly item: number;
    readonly amount: number;
    readonly meta: ItemMeta;
}