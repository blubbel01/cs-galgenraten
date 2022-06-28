import {ItemStack} from "./ItemStack";

export interface Inventory {
    readonly title: string;
    readonly maxWeight: number;
    readonly items: ItemStack[];
}