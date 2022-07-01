

CREATE TABLE `inventories` (
                               `id` bigint(255) NOT NULL,
                               `title` varchar(255) NOT NULL,
                               `maxWeight` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `inventory_attributes` (
                                        `inventory_id` bigint(20) NOT NULL,
                                        `attribute` int(11) NOT NULL,
                                        `value` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `items` (
                         `id` bigint(255) NOT NULL,
                         `name` varchar(255) NOT NULL,
                         `description` text DEFAULT NULL,
                         `weight` double NOT NULL DEFAULT 0,
                         `legal` tinyint(1) NOT NULL DEFAULT 1,
                         `disabled` tinyint(1) NOT NULL DEFAULT 0,
                         `durability` smallint(6) NOT NULL DEFAULT 0,
                         `heal` int(11) NOT NULL DEFAULT 0,
                         `food` int(11) NOT NULL DEFAULT 0,
                         `priceMin` int(11) NOT NULL DEFAULT 0,
                         `priceMax` int(11) NOT NULL DEFAULT 0,
                         `allowTrade` tinyint(1) NOT NULL DEFAULT 1,
                         `sync` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `itemstacks` (
                              `index` bigint(255) NOT NULL,
                              `inventory_id` bigint(255) NOT NULL,
                              `item_id` bigint(255) NOT NULL,
                              `amount` bigint(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `itemstack_attributes` (
                                        `inventory_id` bigint(20) NOT NULL,
                                        `itemmeta_itemstack_index` bigint(255) NOT NULL,
                                        `attribute` int(11) NOT NULL,
                                        `value` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `itemstack_metas` (
                                   `inventory_id` bigint(20) NOT NULL,
                                   `itemstack_index` bigint(255) NOT NULL,
                                   `displayName` varchar(255) DEFAULT NULL,
                                   `lore` varchar(255) DEFAULT NULL,
                                   `flags` int(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `item_count` (
                              `id` bigint(255)
    ,`name` varchar(255)
    ,`amount` decimal(65,0)
);
DROP TABLE IF EXISTS `item_count`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `item_count`  AS SELECT `items`.`id` AS `id`, `items`.`name` AS `name`, sum(`itemstacks`.`amount`) AS `amount` FROM (`items` join `itemstacks` on(`itemstacks`.`item_id` = `items`.`id`))  ;


ALTER TABLE `inventories`
    ADD PRIMARY KEY (`id`);

ALTER TABLE `inventory_attributes`
    ADD PRIMARY KEY (`inventory_id`,`attribute`),
  ADD KEY `inventory_id` (`inventory_id`);

ALTER TABLE `items`
    ADD PRIMARY KEY (`id`);

ALTER TABLE `itemstacks`
    ADD PRIMARY KEY (`index`,`inventory_id`),
  ADD KEY `itemstack_inventory_id` (`inventory_id`),
  ADD KEY `itemstack_type` (`item_id`);

ALTER TABLE `itemstack_attributes`
    ADD PRIMARY KEY (`inventory_id`,`itemmeta_itemstack_index`,`attribute`),
  ADD KEY `attributemodifiers_itemmeta_id` (`itemmeta_itemstack_index`),
  ADD KEY `itemstack_attributes_itemstack_metas` (`itemmeta_itemstack_index`,`inventory_id`);

ALTER TABLE `itemstack_metas`
    ADD PRIMARY KEY (`inventory_id`,`itemstack_index`),
  ADD KEY `itemmeta_itemstack_id` (`itemstack_index`),
  ADD KEY `itemstack_metas_itemstack` (`itemstack_index`,`inventory_id`);


ALTER TABLE `inventories`
    MODIFY `id` bigint(255) NOT NULL AUTO_INCREMENT;

ALTER TABLE `items`
    MODIFY `id` bigint(255) NOT NULL AUTO_INCREMENT;


ALTER TABLE `inventory_attributes`
    ADD CONSTRAINT `inventory_attributes_inventory_id` FOREIGN KEY (`inventory_id`) REFERENCES `inventories` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `itemstacks`
    ADD CONSTRAINT `itemstack_inventory_id` FOREIGN KEY (`inventory_id`) REFERENCES `inventories` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `itemstack_type` FOREIGN KEY (`item_id`) REFERENCES `items` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `itemstack_attributes`
    ADD CONSTRAINT `itemstack_attributes_itemstack_metas` FOREIGN KEY (`itemmeta_itemstack_index`,`inventory_id`) REFERENCES `itemstack_metas` (`itemstack_index`, `inventory_id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `itemstack_metas`
    ADD CONSTRAINT `itemstack_metas_itemstack` FOREIGN KEY (`itemstack_index`,`inventory_id`) REFERENCES `itemstacks` (`index`, `inventory_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;
