CREATE TABLE `items` (
                         `id` bigint(11) NOT NULL,
                         `name` varchar(255) NOT NULL,
                         `weight` double NOT NULL,
                         `legal` tinyint(1) NOT NULL,
                         `description` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `item_metas` (
                              `id` bigint(11) NOT NULL,
                              `script_key` varchar(255) NOT NULL,
                              `script_value` varchar(255) DEFAULT NULL,
                              `storages_items_id` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `storages` (
                            `id` bigint(20) NOT NULL,
                            `max_size` double NOT NULL,
                            `types_id` bigint(20) NOT NULL,
                            `reference_id` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `storages_items` (
                                  `id` bigint(20) NOT NULL,
                                  `items_id` bigint(20) NOT NULL,
                                  `storage_id` bigint(20) NOT NULL,
                                  `amount` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `storage_types` (
                                 `id` bigint(20) NOT NULL,
                                 `name` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


ALTER TABLE `items`
    ADD PRIMARY KEY (`id`);

ALTER TABLE `item_metas`
    ADD PRIMARY KEY (`id`),
    ADD KEY `storages_items_id` (`storages_items_id`);

ALTER TABLE `storages`
    ADD PRIMARY KEY (`id`),
    ADD KEY `types_id` (`types_id`);

ALTER TABLE `storages_items`
    ADD PRIMARY KEY (`id`),
    ADD KEY `storage_id` (`storage_id`),
    ADD KEY `items_id` (`items_id`);

ALTER TABLE `storage_types`
    ADD PRIMARY KEY (`id`);


ALTER TABLE `items`
    MODIFY `id` bigint(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `item_metas`
    MODIFY `id` bigint(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `storages`
    MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

ALTER TABLE `storages_items`
    MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

ALTER TABLE `storage_types`
    MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;


ALTER TABLE `item_metas`
    ADD CONSTRAINT `item_metas_ibfk_1` FOREIGN KEY (`storages_items_id`) REFERENCES `storages_items` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `storages`
    ADD CONSTRAINT `storages_ibfk_1` FOREIGN KEY (`types_id`) REFERENCES `storage_types` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE `storages_items`
    ADD CONSTRAINT `storages_items_ibfk_1` FOREIGN KEY (`items_id`) REFERENCES `items` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
    ADD CONSTRAINT `storages_items_ibfk_2` FOREIGN KEY (`storage_id`) REFERENCES `storages` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

