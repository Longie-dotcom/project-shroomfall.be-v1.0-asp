namespace Domain.Shared
{
    public static class ResponseCode
    {
        #region API
        #endregion

        #region Application
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // HELPER
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Claim Reader
        // ─────────────────────────────
        public const string ClaimReader_ClaimMissingUserId = "claim_reader.claim_missing_user_id";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // FEATURE
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Identity Features 
        // ─────────────────────────────
        public const string RefreshToken_UserNotFound = "refresh_token.user_not_found";
        public const string Register_EmailAlreadyExists = "register.email_already_exists";
        public const string Register_EmailRequired = "register.email_required";
        public const string Login_EmailRequired = "login.email_required";
        public const string Login_PasswordRequired = "login.password_required";
        public const string Login_InvalidCredentials = "login.invalid_credentials";
        public const string SteamAuth_InvalidSteamTicket = "steam_auth.invalid_steam_ticket";
        public const string SteamAuth_SteamValidationFailed = "steam_auth.steam_validation_failed";
        public const string UpdateProfile_UserNotFound = "update_profile.user_not_found";
        public const string UpdateProfile_LocaleFound = "update_profile.locale_found";

        // ─────────────────────────────
        // Game Features 
        // ─────────────────────────────
        public const string Move_SessionNotFound = "move.session_not_found";
        public const string Move_PlayerInstanceNotFound = "move.player_instance_not_found";

        // ─────────────────────────────
        // Design Features 
        // ─────────────────────────────
        public const string UpdateDefinition_InvalidVersion = "update_definition.invalid_version";

        // ─────────────────────────────
        // Connect Features 
        // ─────────────────────────────
        public const string ChnageRoom_SessionNotFound = "change_room.session_not_found";
        public const string LoadSession_SessionAlreadyExisted = "load_session.session_already_existed";
        public const string UnloadSession_SessionNotFound = "unload_session.session_not_found";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // SERVICE
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Collision Service
        // ─────────────────────────────
        public const string CollisionService_RoomSpatialNotFoundOnQueryMovement = "collision_service.room_spatial_not_found_on_query_movement";
        public const string CollisionService_RoomSpatialNotFoundOnValidateSpawn = "collision_service.room_spatial_not_found_on_validate_spawn";
        public const string CollisionService_SpawnBlockedByEntity = "collision_service.spawn_blocked_by_entity";
        public const string CollisionService_SpawnBlockedByTile = "collision_service.spawn_blocked_by_tile";

        // ─────────────────────────────
        // Characteristic Service 
        // ─────────────────────────────
        public const string CharacteristicService_InvalidNonVitalAttribute = "characteristic_service.invalid_non_vital_attribute";
        public const string CharacteristicService_MissingAttributeValue = "characteristic_service.missing_attribute_value";

        // ─────────────────────────────
        // Spawn Service
        // ─────────────────────────────
        public const string SpawnService_RoomDefinitionNotFound = "spawn_service.room_definition_not_found";
        public const string SpawnService_SpawnNotAllowed = "spawn_service.spawn_not_allowed";
        public const string SpawnService_NoSpawnArea = "spawn_service.no_spawn_area";

        // ─────────────────────────────
        // Initialization Service
        // ─────────────────────────────
        public const string InitializationService_RoomDefinitionNotFound = "initialization_service.room_definition_not_found";

        // ─────────────────────────────
        // Inventory Service
        // ─────────────────────────────
        public const string InventoryService_DefinitionNotFound = "inventory_service.definition_not_found";
        public const string InventoryService_ItemDefinitionNotFound = "inventory_service.item_definition_not_found";
        public const string InventoryService_NoFreeSlot = "inventory_service.no_free_slot";
        public const string InventoryService_ItemNotFound = "inventory_service.item_not_found";
        public const string InventoryService_InvalidEquipItem = "inventory_service.invalid_equip_item";

        // ─────────────────────────────
        // Equipment Service
        // ─────────────────────────────
        public const string EquipmentService_ItemDefinitionNotFound = "equipment_service.item_definition_not_found";
        public const string EquipmentService_InvalidItemType = "equipment_service.invalid_item_type";
        public const string EquipmentService_InvalidItem = "equipment_service.invalid_item";
        public const string EquipmentService_InventoryFullOnUnequip = "equipment_service.inventory_full_on_unequip";
        public const string EquipmentService_ItemNotEquippable = "equipment_service.item_not_equippable";
        public const string EquipmentService_ItemAlreadyEquipped = "equipment_service.item_already_equipped";
        public const string EquipmentService_EquipmentSlotOccupied = "equipment_service.equipment_slot_occupied";
        public const string EquipmentService_UnequipFailed = "equipment_service.unequip_failed";

        // ─────────────────────────────
        // Item Service
        // ─────────────────────────────
        public const string ItemService_ItemNotFoundInInventory = "item_service.item_not_found_in_inventory";
        public const string ItemService_ItemDefinitionNotFound = "item_service.item_definition_not_found";
        public const string ItemService_TypeNotSupported = "item_service.type_not_support";

        // ─────────────────────────────
        // Topology Service
        // ─────────────────────────────
        public const string TopologyService_EntityNotFound = "topology_service.entity_not_found";
        public const string TopologyService_RoomNotFound = "topology_service.room_not_found";
        public const string TopologyService_NoConnectionDefinition = "topology_service.no_connection_definition";
        public const string TopologyService_DestinationEntityMissing = "topology_service.destination_entity_missing";
        
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // COORDINATOR
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Player Coordinator
        // ─────────────────────────────
        public const string PlayerCoordinator_PlayerInstanceNotFoundInPersistence = "player_coordinator.player_instance_not_found_in_persistence";
        public const string PlayerCoordinator_PlayerInstanceNotFoundInRuntime = "player_coordinator.player_instance_not_found_in_runtime";
        public const string PlayerCoordinator_UnauthorizedPlayerInstance = "player_coordinator.unauthorized_player_instance";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // SYSTEM - TICK
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Residency Tick
        // ─────────────────────────────
        public const string ResidencyTick_RoomSpatialNotFoundInRuntime = "residency_tick.room_spatial_not_found_in_runtime";
        public const string ResidencyTick_RoomSnapshotNotFoundInPersistence = "residency_tick.room_snapshot_not_found_in_persistence";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // HELPER
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Entity Mapper
        // ─────────────────────────────
        public const string EntityMapper_InvalidDocumentType = "entity_mapper.invalid_document_type";
        public const string EntityMapper_InvalidRuntimeType = "entity_mapper.invalid_runtime_type";
        #endregion

        #region Domain
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // HELPER
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Collision Shape Mapper
        // ─────────────────────────────
        public const string CollisionShapeMapper_InvalidShapeType = "collision_shape_mapper.invalid_shape_type";

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // *************************************************************************************************** OTHER ***************************************************************************************************
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // VERSION DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Definition Version Log
        // ─────────────────────────────
        public const string DefinitionVersionLog_InvalidId = "definition_version_log.invalid_id";
        public const string DefinitionVersionLog_InvalidKey = "definition_version_log.invalid_key";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // IDENTITY DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // User
        // ─────────────────────────────
        public const string User_InvalidId = "user.invalid_id";
        public const string User_InvalidName = "user.invalid_name";
        public const string User_MissingAuth = "user.missing_auth";
        public const string User_InvalidDob = "user.invalid_dob";
        public const string User_InvalidGender = "user.invalid_gender";
        public const string User_PasswordNotSet = "user.password_not_set";
        public const string User_InvalidCredentials = "user.invalid_credentials";
        public const string User_InvalidRefreshToken = "user.invalid_refresh_token";
        public const string User_ExpiredRefreshToken = "user.expired_refresh_token";
        public const string User_PasswordRequired = "user.password_required";

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ************************************************************************************************* DEFINITION ************************************************************************************************
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // WORLD DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Room
        // ─────────────────────────────
        public const string Room_InvalidId = "room.invalid_id";
        public const string Room_InvalidName = "room.invalid_name";
        public const string Room_InvalidDescription = "room.invalid_description";

        // ─────────────────────────────
        // Tile
        // ─────────────────────────────
        public const string Tile_InvalidId = "tile.invalid_id";

        // ─────────────────────────────
        // Cell
        // ─────────────────────────────
        public const string Cell_InvalidRoomId = "cell.invalid_room_id";
        public const string Cell_InvalidTileId = "cell.invalid_tile_id";

        // ─────────────────────────────
        // Entity Spawn Rule
        // ─────────────────────────────
        public const string EntitySpawnRule_InvalidId = "entity_spawn_rule.invalid_id";
        public const string EntitySpawnRule_InvalidRoomId = "entity_spawn_rule.invalid_room_id";
        public const string EntitySpawnRule_InvalidEntityId = "entity_spawn_rule.invalid_entity_id";

        // ─────────────────────────────
        // Spawn Area
        // ─────────────────────────────
        public const string SpawnArea_InvalidId = "spawn_area.invalid_id";
        public const string SpawnArea_InvalidEntitySpawnRuleId = "spawn_area.invalid_entity_spawn_rule_id";
        public const string SpawnArea_InvalidBounds = "spawn_area.invalid_bounds";
        public const string SpawnArea_InvalidMinCount = "spawn_area.invalid_min_count";
        public const string SpawnArea_InvalidMaxCount = "spawn_area.invalid_max_count";
        public const string SpawnArea_InvalidWeight = "spawn_area.invalid_weight";

        // ─────────────────────────────
        // Room Connection
        // ─────────────────────────────
        public const string RoomConnection_InvalidId = "room_connection.invalid_id";
        public const string RoomConnection_InvalidSourceRoomId = "room_connection.invalid_source_room_id";
        public const string RoomConnection_InvalidSourceEntityId = "room_connection.invalid_source_entity_id";
        public const string RoomConnection_InvalidDestinationRoomId = "room_connection.invalid_destination_room_id";
        public const string RoomConnection_InvalidDestinationEntityId = "room_connection.invalid_destination_entity_id";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ATTRIBUTE DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Attribute Value
        // ─────────────────────────────
        public const string AttributeValue_InvalidCharacteristicId = "attribute_value.invalid_characteristic_id";
        public const string AttributeValue_InvalidValue = "attribute_value.invalid_value";
        public const string AttributeValue_InvalidLevel = "attribute_value.invalid_level";
        public const string AttributeValue_InvalidMin = "attribute_value.invalid_min";
        public const string AttributeValue_InvalidMax = "attribute_value.invalid_max";

        // ─────────────────────────────
        // Characteristic
        // ─────────────────────────────
        public const string Characteristic_InvalidId = "characteristic.invalid_id";
        public const string Characteristic_InvalidName = "characteristic.invalid_name";
        public const string Characteristic_InvalidDescription = "characteristic.invalid_description";

        // ─────────────────────────────
        // Effect
        // ─────────────────────────────
        public const string Effect_InvalidId = "effect.invalid_id";
        public const string Effect_InvalidName = "effect.invalid_name";
        public const string Effect_InvalidDescription = "effect.invalid_description";
        public const string Effect_InvalidValue = "effect.invalid_value";
        public const string Effect_InvalidDuration = "effect.invalid_duration";
        public const string Effect_InvalidInterval = "effect.invalid_interval";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ENTITY DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Entity
        // ─────────────────────────────
        public const string Entity_InvalidId = "entity.invalid_id";
        public const string Entity_InvalidName = "entity.invalid_name";
        public const string Entity_InvalidDescription = "entity.invalid_description";
        public const string Entity_InvalidSkinId = "entity.invalid_skin_Id";

        // ─────────────────────────────
        // Creature
        // ─────────────────────────────
        public const string Creature_InvalidCharacteristicId = "creature.invalid_characteristic_id";
        public const string Creature_InvalidInventoryId = "creature.invalid_inventory_id";
        public const string Creature_InvalidLevel = "creature.invalid_level";

        // ─────────────────────────────
        // Portal
        // ─────────────────────────────
        public const string Portal_InvalidEntrance = "portal.invalid_entrance";
        public const string Portal_EntranceMustBeNonBlocking = "portal.entrance_must_be_non_blocking";
        public const string Portal_EntranceMustBeTrigger = "portal.entrance_must_be_trigger";

        // ─────────────────────────────
        // Collision
        // ─────────────────────────────
        public const string Collision_InvalidShapeType = "collision.invalid_shape_type";
        public const string Collision_InvalidWidth = "collision.invalid_width";
        public const string Collision_InvalidHeight = "collision.invalid_height";
        public const string Collision_InvalidRadius = "collision.invalid_radius";
        public const string Collision_InvalidDimensionForShape = "collision.invalid_dimension_for_shape";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // INVENTORY DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Inventory
        // ─────────────────────────────
        public const string Inventory_InvalidId = "inventory.invalid_id";
        public const string Inventory_InvalidName = "inventory.invalid_name";
        public const string Inventory_InvalidDescription = "inventory.invalid_description";
        public const string Inventory_InvalidSlotCount = "inventory.invalid_slot_count";

        // ─────────────────────────────
        // InventoryItem
        // ─────────────────────────────
        public const string InventoryItem_InvalidInventoryId = "inventory_item.invalid_inventory_id";
        public const string InventoryItem_InvalidItemId = "inventory_item.invalid_item_id";
        public const string InventoryItem_InvalidAmount = "inventory_item.invalid_amount";

        // ─────────────────────────────
        // Item
        // ─────────────────────────────
        public const string Item_InvalidId = "item.invalid_id";
        public const string Item_InvalidName = "item.invalid_name";
        public const string Item_InvalidDescription = "item.invalid_description";
        public const string Item_InvalidDurability = "item.invalid_durability";
        public const string Item_InvalidStackableDurability = "item.invalid_stackable_durability";

        // ─────────────────────────────
        // Item Configuration
        // ─────────────────────────────
        public const string ItemConfiguration_InvalidId = "item_configuration.invalid_id";
        public const string ItemConfiguration_InvalidItemId = "item_configuration.invalid_item_id";
        public const string ItemConfiguration_InvalidEntityId = "item_configuration.invalid_entity_id";

        // ─────────────────────────────
        // Item Effect
        // ─────────────────────────────
        public const string ItemEffect_InvalidItemId = "item_effect.invalid_item_id";
        public const string ItemEffect_InvalidEffectId = "item_effect.invalid_effect_id";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // LOCALIZATION DOMAIN
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Locale
        // ─────────────────────────────
        public const string Locale_InvalidCode = "locale.invalid_code";
        public const string Locale_InvalidName = "locale.invalid_name";
        public const string Locale_CanNotDisableDefault = "locale.can_not_disable_default";

        // ─────────────────────────────
        // Localization Entry
        // ─────────────────────────────
        public const string LocalizationEntry_InvalidId = "localization_entry.invalid_id";
        public const string LocalizationEntry_InvalidKey = "localization_entry.invalid_key";
        public const string LocalizationEntry_InvalidLocaleCode = "localization_entry.invalid_locale_code";
        public const string LocalizationEntry_InvalidValue = "localization_entry.invalid_value";

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ************************************************************************************************** RUNTIME **************************************************************************************************
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // World
        // ─────────────────────────────
        public const string World_EntityInstanceNotFoundOnRemoved = "world.entity_instance_not_found_on_removed";
        public const string World_EntityInstanceNotFoundOnMoved = "world.entity_instance_not_found_on_moved";
        public const string World_EntityInstanceNotFoundOnRoomChanged = "world.entity_instance_not_found_on_room_changed";

        // ─────────────────────────────
        // Spatial Index
        // ─────────────────────────────
        public const string SpatialIndex_RoomSpatialNotFoundOnEntityAdded = "spatial_index.room_spatial_not_found_on_entity_added";
        public const string SpatialIndex_RoomSpatialNotFoundOnEntityRemoved = "spatial_index.room_spatial_not_found_on_entity_removed";
        public const string SpatialIndex_RoomSpatialNotFoundOnEntityMoved = "spatial_index.room_spatial_not_found_on_entity_moved";

        // ─────────────────────────────
        // Characteristic Instance 
        // ─────────────────────────────
        public const string CharacteristicInstance_NotAVitalAttribute = "characteristic_instance.not_a_vital_attribute";
        public const string CharacteristicInstance_NotACoreAttribute = "characteristic_instance.not_a_core_attribute";

        // ─────────────────────────────
        // Item Instance 
        // ─────────────────────────────
        public const string Item_InvalidAmount = "item.invalid_amount";
        public const string Item_NotEnoughAmount = "item.not_enough_item";

        // ─────────────────────────────
        // Room Connection Instance
        // ─────────────────────────────
        public const string RoomConnectionInstance_InvalidDestinationRoomSpatialId = "room_connection_instance.invalid_destination_room_spatial_id";
        public const string RoomConnectionInstance_InvalidDestinationEntityInstanceId = "room_connection_instance.invalid_destination_entity_instance_id";
        #endregion

        #region Infrastructure
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // REPOSITORY
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // RelationalUoW
        // ─────────────────────────────
        public const string RelationalUoW_CommitException = "relational_uow.commit_exception";
        public const string RelationalUoW_NoTransactionCreated = "relational_uow.no_transaction_created";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // FACTORY
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────
        // Player Instance Factory
        // ─────────────────────────────
        public const string PlayerInstanceFactory_DefinitionNotFound = "player_instance_factory.definition_not_found";
        public const string PlayerInstanceFactory_DocumentNotFound = "player_instance_factory.document_not_found";
        public const string PlayerInstanceFactory_DefinitionFromDocumentNotFound = "player_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Characteristic Instance Factory
        // ─────────────────────────────
        public const string CharacteristicInstanceFactory_DefinitionNotFound = "characteristic_instance_factory.definition_not_found";
        public const string CharacteristicInstanceFactory_DocumentNotFound = "characteristic_instance_factory.document_not_found";
        public const string CharacteristicInstanceFactory_DefinitionFromDocumentNotFound = "characteristic_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Inventory Instance Factory
        // ─────────────────────────────
        public const string InventoryInstanceFactory_DefinitionNotFound = "inventory_instance_factory.definition_not_found";
        public const string InventoryInstanceFactory_DocumentNotFound = "inventory_instance_factory.document_not_found";
        public const string InventoryInstanceFactory_DefinitionFromDocumentNotFound = "inventory_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Creature Instance Factory
        // ─────────────────────────────
        public const string CreatureInstanceFactory_DefinitionNotFound = "creature_instance_factory.definition_not_found";
        public const string CreatureInstanceFactory_DocumentNotFound = "creature_instance_factory.document_not_found";
        public const string CreatureInstanceFactory_DefinitionFromDocumentNotFound = "creature_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Creature Instance Factory
        // ─────────────────────────────
        public const string WorldObjectInstanceFactory_DefinitionNotFound = "world_object_instance_factory.definition_not_found";
        public const string WorldObjectInstanceFactory_DocumentNotFound = "world_object_instance_factory.document_not_found";
        public const string WorldObjectInstanceFactory_DefinitionFromDocumentNotFound = "world_object_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Item Factory
        // ─────────────────────────────
        public const string ItemInstanceFactory_DefinitionNotFound = "item_instance_factory.definition_not_found";
        public const string ItemInstanceFactory_DocumentNotFound = "item_instance_factory.document_not_found";
        public const string ItemInstanceFactory_DefinitionFromDocumentNotFound = "item_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Effect Instance Factory
        // ─────────────────────────────
        public const string EffectInstanceFactory_DefinitionNotFound = "effect_instance_factory.definition_not_found";
        public const string EffectInstanceFactory_DocumentNotFound = "effect_instance_factory.document_not_found";
        public const string EffectInstanceFactory_DefinitionFromDocumentNotFound = "effect_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Room Spatial Factory
        // ─────────────────────────────
        public const string RoomSpatialFactory_DefinitionNotFound = "room_spatial_factory.definition_not_found";
        public const string RoomSpatialFactory_RoomWithoutCells = "room_spatial_factory.room_without_cells";
        public const string RoomSpatialFactory_DocumentNotFound = "room_spatial_factory.document_not_found";
        public const string RoomSpatialFactory_DefinitionFromDocumentNotFound = "room_spatial_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Room Connection Instance Factory
        // ─────────────────────────────
        public const string RoomConnectionInstanceFactory_DefinitionNotFound = "room_connection_instance_factory.definition_not_found";
        public const string RoomConnectionInstanceFactory_InvalidInstanceData = "room_connection_instance_factory.invalid_instance_data";
        public const string RoomConnectionInstanceFactory_DocumentNull = "room_connection_instance_factory.document_null";
        public const string RoomConnectionInstanceFactory_DefinitionFromDocumentNotFound = "room_connection_instance_factory.definition_from_document_not_found";

        // ─────────────────────────────
        // Entity Instance Factory
        // ─────────────────────────────
        public const string EntityInstanceFactory_UnknownDocumentType = "room_spatial_factory.unknown_document_type";

        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // HELPER
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        public const string HubContextHelper_UserIdNotFound = "hub_context_helper.user_id_not_found";
        public const string HubContextHelper_ConnectionIdNotFound = "hub_context_helper.connection_id_not_found";
        #endregion
    }
}