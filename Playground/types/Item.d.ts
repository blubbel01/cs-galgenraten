/**
 * hier das enum aus dem ServerCode einfügen
 */
export enum Item {
    NULL = 0,
    TEST_MATERIAL = 1,
    WEAPON_SMG = 12,
}

export interface ItemObject {
    readonly id: number;
    readonly name: string;
    readonly description: string;
    readonly weight: number;
    readonly durability: number;
    readonly legal: boolean;
    readonly disabled: boolean;
    readonly heal: number;
    readonly food: number;
    readonly priceMin: number;
    readonly priceMax: number;
    readonly allowTrade: boolean;
    readonly sync: boolean;
}