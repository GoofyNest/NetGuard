# Packet filter catalog

| Context | Opcode | Filter / structure | Behavior |
|---|---|---|---|
| Agent Client | `0x2001` | [GlobalIdentificationRequest](Agent/Client/Authentication/GlobalIdentificationRequest.cs) | Preserved |
| Agent Client | `0x2002` | [KeepAliveRequest](Disabled/Agent/Client/Connection/KeepAliveRequest.cs) | Default stub |
| Agent Client | `0x3012` | [CharacterReadyRequest](Disabled/Agent/Client/Character/CharacterReadyRequest.cs) | Default stub |
| Agent Client | `0x3014` | [SpoofLocaleVersion](Agent/Client/Game/SpoofLocaleVersion.cs) | Preserved |
| Agent Client | `0x3053` | [CharacterResurrectRequest](Disabled/Agent/Client/Character/CharacterResurrectRequest.cs) | Default stub |
| Agent Client | `0x306E` | [PartyMatchingAnswerRequest](Disabled/Agent/Client/Party/PartyMatchingAnswerRequest.cs) | Default stub |
| Agent Client | `0x3080` | [AcceptanceRequest](Disabled/Agent/Client/Game/AcceptanceRequest.cs) | Default stub |
| Agent Client | `0x308D` | [PartyMatchingAnswerRequestNew](Disabled/Agent/Client/Party/PartyMatchingAnswerRequestNew.cs) | Default stub |
| Agent Client | `0x3091` | [EmoticonRequest](Disabled/Agent/Client/Game/EmoticonRequest.cs) | Default stub |
| Agent Client | `0x30D4` | [ShoppingManagerGetQuestIdRequest](Disabled/Agent/Client/Quest/ShoppingManagerGetQuestIdRequest.cs) | Default stub |
| Agent Client | `0x34A9` | [MagicOptionGrant](Agent/Client/Avatar/MagicOptionGrant.cs) | Preserved |
| Agent Client | `0x34B6` | [GameResetCompleteRequest](Disabled/Agent/Client/Game/GameResetCompleteRequest.cs) | Default stub |
| Agent Client | `0x5000` | [HandshakeRequest](Agent/Client/Connection/HandshakeRequest.cs) | Preserved |
| Agent Client | `0x6103` | [AgentLoginRequestHook](Agent/Client/Authentication/AgentLoginRequestHook.cs) | Preserved |
| Agent Client | `0x6118` | [AgentLoginRequestHookRigid](Disabled/Agent/Client/Authentication/AgentLoginRequestHookRigid.cs) | Default stub |
| Agent Client | `0x7001` | [CharacterSelectionJoinRequest](Disabled/Agent/Client/CharacterSelection/CharacterSelectionJoinRequest.cs) | Default stub |
| Agent Client | `0x7005` | [LogoutRequest](Agent/Client/Logout/LogoutRequest.cs) | Preserved |
| Agent Client | `0x7007` | [CharacterListRequest](Disabled/Agent/Client/Character/CharacterListRequest.cs) | Default stub |
| Agent Client | `0x7010` | [GMCommand](Agent/Client/Operator/GMCommand.cs) | Preserved |
| Agent Client | `0x7021` | [PlayerMovement](Disabled/Agent/Client/Character/PlayerMovement.cs) | Preserved |
| Agent Client | `0x7025` | [ChatRequestHook](Agent/Client/Chat/ChatRequestHook.cs) | Preserved |
| Agent Client | `0x7034` | [InventoryOperationRequest](Disabled/Agent/Client/Inventory/InventoryOperationRequest.cs) | Preserved |
| Agent Client | `0x703C` | [StorageOpenRequest](Disabled/Agent/Client/Inventory/StorageOpenRequest.cs) | Default stub |
| Agent Client | `0x703E` | [InventoryItemRepairRequest](Disabled/Agent/Client/Inventory/InventoryItemRepairRequest.cs) | Default stub |
| Agent Client | `0x7045` | [ActionSelectRequestHook](Disabled/Agent/Client/Action/ActionSelectRequestHook.cs) | Default stub |
| Agent Client | `0x7046` | [ActionTalkRequest](Disabled/Agent/Client/Action/ActionTalkRequest.cs) | Default stub |
| Agent Client | `0x704B` | [ActionDeselectRequest](Disabled/Agent/Client/Action/ActionDeselectRequest.cs) | Default stub |
| Agent Client | `0x704C` | [InventoryItemUseRequest](Agent/Client/Inventory/InventoryItemUseRequest.cs) | Preserved |
| Agent Client | `0x704F` | [SitdownRequest](Disabled/Agent/Client/Game/SitdownRequest.cs) | Default stub |
| Agent Client | `0x7050` | [CharacterIncreaseStrRequest](Disabled/Agent/Client/Character/CharacterIncreaseStrRequest.cs) | Default stub |
| Agent Client | `0x7051` | [CharacterIncreaseIntRequest](Disabled/Agent/Client/Character/CharacterIncreaseIntRequest.cs) | Default stub |
| Agent Client | `0x705A` | [TeleportRequest](Agent/Client/Teleport/TeleportRequest.cs) | Preserved |
| Agent Client | `0x705C` | [CosActionRequestHook](Disabled/Agent/Client/Cos/CosActionRequestHook.cs) | Default stub |
| Agent Client | `0x705E` | [FortressSiegeAction](Agent/Client/Siege/FortressSiegeAction.cs) | Preserved |
| Agent Client | `0x7060` | [PartyInviteRequest](Disabled/Agent/Client/Party/PartyInviteRequest.cs) | Default stub |
| Agent Client | `0x7061` | [PartyLeaveRequest](Disabled/Agent/Client/Party/PartyLeaveRequest.cs) | Default stub |
| Agent Client | `0x7062` | [PartyInviteRequest_7062](Disabled/Agent/Client/Party/PartyInviteRequest_7062.cs) | Default stub |
| Agent Client | `0x7063` | [PartyMemberBanishRequest](Disabled/Agent/Client/Party/PartyMemberBanishRequest.cs) | Default stub |
| Agent Client | `0x7069` | [PartyMatchingFormRequest](Disabled/Agent/Client/Party/PartyMatchingFormRequest.cs) | Default stub |
| Agent Client | `0x706A` | [PartyMatchingChangeRequest](Disabled/Agent/Client/Party/PartyMatchingChangeRequest.cs) | Default stub |
| Agent Client | `0x706B` | [PartyMatchingDeleteRequest](Disabled/Agent/Client/Party/PartyMatchingDeleteRequest.cs) | Default stub |
| Agent Client | `0x706C` | [PartyMatchingListRequest](Disabled/Agent/Client/Party/PartyMatchingListRequest.cs) | Default stub |
| Agent Client | `0x706D` | [PartyMatchingJoinRequest](Disabled/Agent/Client/Party/PartyMatchingJoinRequest.cs) | Default stub |
| Agent Client | `0x7074` | [ActionCommandRequest](Agent/Client/Game/ActionCommandRequest.cs) | Preserved |
| Agent Client | `0x70A2` | [SkillMasteryLearn](Agent/Client/Skills/SkillMasteryLearn.cs) | Preserved |
| Agent Client | `0x70A7` | [PlayerActivateBerserk](Agent/Client/Character/PlayerActivateBerserk.cs) | Preserved |
| Agent Client | `0x70C5` | [CosCommandRequest](Disabled/Agent/Client/Cos/CosCommandRequest.cs) | Default stub |
| Agent Client | `0x70C6` | [CosTerminate](Disabled/Agent/Client/Cos/CosTerminate.cs) | Preserved |
| Agent Client | `0x70CB` | [CosMountRequest](Disabled/Agent/Client/Cos/CosMountRequest.cs) | Default stub |
| Agent Client | `0x70D9` | [QuestLogAbandonQuestRequest](Disabled/Agent/Client/Quest/QuestLogAbandonQuestRequest.cs) | Default stub |
| Agent Client | `0x70F9` | [GuildUpdateNotice](Agent/Client/Guild/GuildUpdateNotice.cs) | Preserved |
| Agent Client | `0x7116` | [CosTerminateRequest](Agent/Client/Cos/CosTerminateRequest.cs) | Default stub |
| Agent Client | `0x7150` | [ElixirFuseRequestHandler](Disabled/Agent/Client/Alchemy/ElixirFuseRequestHandler.cs) | Default stub |
| Agent Client | `0x7151` | [StoneFuseRequestHandler](Disabled/Agent/Client/Alchemy/StoneFuseRequestHandler.cs) | Default stub |
| Agent Client | `0x7158` | [HotkeyUpdate](Disabled/Agent/Client/Character/HotkeyUpdate.cs) | Preserved |
| Agent Client | `0x7202` | [SkillWithdrawRequest](Disabled/Agent/Client/Skill/SkillWithdrawRequest.cs) | Default stub |
| Agent Client | `0x7250` | [ShoppingManagerOpenGuildStorageRequest](Disabled/Agent/Client/Guild/ShoppingManagerOpenGuildStorageRequest.cs) | Default stub |
| Agent Client | `0x7251` | [ShoppingManagerCloseGuildStorageRequest](Disabled/Agent/Client/Guild/ShoppingManagerCloseGuildStorageRequest.cs) | Default stub |
| Agent Client | `0x7252` | [GuildStorageOpenFollowupRequest](Disabled/Agent/Client/Guild/GuildStorageOpenFollowupRequest.cs) | Default stub |
| Agent Client | `0x7461` | [StallNetwork](Disabled/Agent/Client/Character/StallNetwork.cs) | Preserved |
| Agent Client | `0x7515` | [ShoppingManagerReceiveQuestRewardRequest](Disabled/Agent/Client/Quest/ShoppingManagerReceiveQuestRewardRequest.cs) | Default stub |
| Agent Client | `0x9000` | [HandshakeAcceptRequest](Agent/Client/Connection/HandshakeAcceptRequest.cs) | Preserved |
| Agent Server | `0x2001` | [GlobalIdentificationRequest](Disabled/Agent/Server/Authentication/GlobalIdentificationRequest.cs) | Default stub |
| Agent Server | `0x300A` | [LogoutSuccessResponse](Disabled/Agent/Server/Logout/LogoutSuccessResponse.cs) | Default stub |
| Agent Server | `0x300C` | [AgentNotifyResponse](Agent/Server/Cos/AgentNotifyResponse.cs) | Default stub |
| Agent Server | `0x3012` | [TeleportCompleteResponse](Disabled/Agent/Server/Teleport/TeleportCompleteResponse.cs) | Default stub |
| Agent Server | `0x3013` | [CharacterDataResponse](Agent/Server/Character/CharacterDataResponse.cs) | Preserved |
| Agent Server | `0x3015` | [EntitySingleSpawnResponse](Agent/Server/Entity/EntitySingleSpawnResponse.cs) | Preserved |
| Agent Server | `0x3016` | [EntitySingleDespawnResponse](Disabled/Agent/Server/Entity/EntitySingleDespawnResponse.cs) | Default stub |
| Agent Server | `0x3017` | [EntityGroupSpawnBeginResponse](Disabled/Agent/Server/Entity/EntityGroupSpawnBeginResponse.cs) | Default stub |
| Agent Server | `0x3018` | [EntityGroupSpawnEndResponse](Disabled/Agent/Server/Entity/EntityGroupSpawnEndResponse.cs) | Default stub |
| Agent Server | `0x3019` | [EntityGroupSpawnDataResponse](Disabled/Agent/Server/Entity/EntityGroupSpawnDataResponse.cs) | Default stub |
| Agent Server | `0x3020` | [EnviromentCelestialPosition](Agent/Server/Authentication/EnviromentCelestialPosition.cs) | Preserved |
| Agent Server | `0x3026` | [ChatResponse](Disabled/Agent/Server/Chat/ChatResponse.cs) | Default stub |
| Agent Server | `0x3028` | [EntitySourcePositionUpdate](Disabled/Agent/Server/Entity/EntitySourcePositionUpdate.cs) | Default stub |
| Agent Server | `0x303D` | [CharacterUpdateStatsResponse](Disabled/Agent/Server/Character/CharacterUpdateStatsResponse.cs) | Default stub |
| Agent Server | `0x3040` | [InventoryUpdateItemResponse](Disabled/Agent/Server/Inventory/InventoryUpdateItemResponse.cs) | Default stub |
| Agent Server | `0x3047` | [InventoryStorageDataBeginResponse](Disabled/Agent/Server/Inventory/InventoryStorageDataBeginResponse.cs) | Default stub |
| Agent Server | `0x3048` | [InventoryStorageDataEndResponse](Disabled/Agent/Server/Inventory/InventoryStorageDataEndResponse.cs) | Default stub |
| Agent Server | `0x3049` | [InventoryStorageDataResponse](Disabled/Agent/Server/Inventory/InventoryStorageDataResponse.cs) | Default stub |
| Agent Server | `0x304D` | [EntityRemoveOwnershipResponse](Disabled/Agent/Server/Entity/EntityRemoveOwnershipResponse.cs) | Default stub |
| Agent Server | `0x304E` | [EntityUpdatePointsResponse](Disabled/Agent/Server/Entity/EntityUpdatePointsResponse.cs) | Default stub |
| Agent Server | `0x3052` | [InventoryUpdateDurabilityResponse](Disabled/Agent/Server/Inventory/InventoryUpdateDurabilityResponse.cs) | Default stub |
| Agent Server | `0x3054` | [EntityAnimationPromoteResponse](Disabled/Agent/Server/Entity/EntityAnimationPromoteResponse.cs) | Default stub |
| Agent Server | `0x3056` | [EntityUpdateExperienceResponse](Disabled/Agent/Server/Entity/EntityUpdateExperienceResponse.cs) | Default stub |
| Agent Server | `0x3057` | [EntityUpdateStatusResponse](Disabled/Agent/Server/Entity/EntityUpdateStatusResponse.cs) | Default stub |
| Agent Server | `0x305C` | [AnimationUseItem](Disabled/Agent/Server/Game/AnimationUseItem.cs) | Preserved |
| Agent Server | `0x3065` | [PartyCreateFromMatching](Disabled/Agent/Server/Party/PartyCreateFromMatching.cs) | Default stub |
| Agent Server | `0x3068` | [PartyDistributionResponse](Disabled/Agent/Server/Party/PartyDistributionResponse.cs) | Default stub |
| Agent Server | `0x3077` | [BuffTokenUpdateResponse](Disabled/Agent/Server/Game/BuffTokenUpdateResponse.cs) | Default stub |
| Agent Server | `0x3080` | [PartyInviteResponse](Disabled/Agent/Server/Party/PartyInviteResponse.cs) | Default stub |
| Agent Server | `0x3085` | [ExchangeStartedResponse](Agent/Server/Exchange/ExchangeStartedResponse.cs) | Preserved |
| Agent Server | `0x3087` | [ExchangeApprovedResponse](Disabled/Agent/Server/Exchange/ExchangeApprovedResponse.cs) | Default stub |
| Agent Server | `0x3088` | [ExchangeCanceledResponse](Disabled/Agent/Server/Exchange/ExchangeCanceledResponse.cs) | Default stub |
| Agent Server | `0x308C` | [ExchangeUpdateItemsResponse](Disabled/Agent/Server/Exchange/ExchangeUpdateItemsResponse.cs) | Default stub |
| Agent Server | `0x3092` | [InventoryUpdateSizeResponse](Disabled/Agent/Server/Inventory/InventoryUpdateSizeResponse.cs) | Default stub |
| Agent Server | `0x30BF` | [EntityUpdateStateResponse](Disabled/Agent/Server/Entity/EntityUpdateStateResponse.cs) | Default stub |
| Agent Server | `0x30C8` | [CosDataResponse](Agent/Server/Cos/CosDataResponse.cs) | Preserved |
| Agent Server | `0x30C9` | [CosUpdateResponse](Agent/Server/Cos/CosUpdateResponse.cs) | Preserved |
| Agent Server | `0x30D0` | [EntityUpdateMoveSpeedResponse](Disabled/Agent/Server/Entity/EntityUpdateMoveSpeedResponse.cs) | Default stub |
| Agent Server | `0x30D5` | [QuestUpdateResponse](Disabled/Agent/Server/Quest/QuestUpdateResponse.cs) | Default stub |
| Agent Server | `0x30E0` | [JobUpdatePriceResponse](Disabled/Agent/Server/Job/JobUpdatePriceResponse.cs) | Default stub |
| Agent Server | `0x30E6` | [JobUpdateExperienceResponse](Disabled/Agent/Server/Job/JobUpdateExperienceResponse.cs) | Default stub |
| Agent Server | `0x30E7` | [JobCosStuckResponse](Disabled/Agent/Server/Job/JobCosStuckResponse.cs) | Default stub |
| Agent Server | `0x30E8` | [JobUpdateTradeScaleResponse](Disabled/Agent/Server/Job/JobUpdateTradeScaleResponse.cs) | Default stub |
| Agent Server | `0x3123` | [FatigueTimeUpdateHandler](Disabled/Agent/Server/Game/FatigueTimeUpdateHandler.cs) | Default stub |
| Agent Server | `0x3153` | [SilkUpdate](Agent/Server/Silk/SilkUpdate.cs) | Preserved |
| Agent Server | `0x3201` | [InventoryUpdateAmmoResponse](Disabled/Agent/Server/Inventory/InventoryUpdateAmmoResponse.cs) | Default stub |
| Agent Server | `0x3253` | [InventoryGuildStorageDataBeginResponse](Disabled/Agent/Server/Inventory/InventoryGuildStorageDataBeginResponse.cs) | Default stub |
| Agent Server | `0x3254` | [InventoryGuildStorageDataEndResponse](Disabled/Agent/Server/Inventory/InventoryGuildStorageDataEndResponse.cs) | Default stub |
| Agent Server | `0x3255` | [InventoryGuildStorageDataResponse](Disabled/Agent/Server/Inventory/InventoryGuildStorageDataResponse.cs) | Default stub |
| Agent Server | `0x325F` | [ActionItemPerkAddResponse](Disabled/Agent/Server/Action/ActionItemPerkAddResponse.cs) | Default stub |
| Agent Server | `0x3261` | [ActionItemPerkRemoveResponse](Disabled/Agent/Server/Action/ActionItemPerkRemoveResponse.cs) | Default stub |
| Agent Server | `0x3422` | [FellowStatUpdateResponse](Agent/Server/Cos/FellowStatUpdateResponse.cs) | Default stub |
| Agent Server | `0x34A5` | [CharacterDataBeginResponse](Agent/Server/Character/CharacterDataBeginResponse.cs) | Preserved |
| Agent Server | `0x34A6` | [CharacterDataEndResponse](Agent/Server/Character/CharacterDataEndResponse.cs) | Preserved |
| Agent Server | `0x34A9` | [MagicOptionGrantResponse](Disabled/Agent/Server/Alchemy/MagicOptionGrantResponse.cs) | Default stub |
| Agent Server | `0x34AA` | [MagicOptionUpdateResponse](Disabled/Agent/Server/Alchemy/MagicOptionUpdateResponse.cs) | Default stub |
| Agent Server | `0x34B5` | [GameResetCompleteResponse](Disabled/Agent/Server/Game/GameResetCompleteResponse.cs) | Default stub |
| Agent Server | `0x3514` | [ShoppingManagerGetQuestIdResponse](Disabled/Agent/Server/Quest/ShoppingManagerGetQuestIdResponse.cs) | Default stub |
| Agent Server | `0x3864` | [PartyUpdateResponse](Disabled/Agent/Server/Party/PartyUpdateResponse.cs) | Default stub |
| Agent Server | `0x3E6E` | [PartyUpdateResponseNew](Disabled/Agent/Server/Party/PartyUpdateResponseNew.cs) | Default stub |
| Agent Server | `0x5000` | [HandshakeResponse](Agent/Server/Connection/HandshakeResponse.cs) | Preserved |
| Agent Server | `0x706D` | [PartyMatchingInviteRequest](Disabled/Agent/Server/Party/PartyMatchingInviteRequest.cs) | Default stub |
| Agent Server | `0x9000` | [HandshakeAcceptResponse](Disabled/Agent/Server/Connection/HandshakeAcceptResponse.cs) | Default stub |
| Agent Server | `0xA103` | [AgentLoginResponse](Agent/Server/Authentication/AgentLoginResponse.cs) | Preserved |
| Agent Server | `0xA118` | [AgentLoginResponseRigid](Disabled/Agent/Server/Authentication/AgentLoginResponseRigid.cs) | Default stub |
| Agent Server | `0xB001` | [CharacterSelectionResponse](Agent/Server/CharacterSelection/CharacterSelectionResponse.cs) | Preserved |
| Agent Server | `0xB007` | [CharacterListing](Agent/Server/Character/CharacterListing.cs) | Preserved |
| Agent Server | `0xB021` | [EntityUpdateMovementResponse](Disabled/Agent/Server/Entity/EntityUpdateMovementResponse.cs) | Default stub |
| Agent Server | `0xB023` | [EntityUpdatePositionResponse](Disabled/Agent/Server/Entity/EntityUpdatePositionResponse.cs) | Default stub |
| Agent Server | `0xB024` | [EntityUpdateAngleResponse](Disabled/Agent/Server/Entity/EntityUpdateAngleResponse.cs) | Default stub |
| Agent Server | `0xB025` | [ChatResponseHook](Disabled/Agent/Server/Chat/ChatResponseHook.cs) | Default stub |
| Agent Server | `0xB034` | [InventoryOperationResponse](Disabled/Agent/Server/Inventory/InventoryOperationResponse.cs) | Default stub |
| Agent Server | `0xB03E` | [ShoppingManagerRepairItemsResponse](Disabled/Agent/Server/Game/ShoppingManagerRepairItemsResponse.cs) | Default stub |
| Agent Server | `0xB045` | [ActionSelectResponse](Disabled/Agent/Server/Action/ActionSelectResponse.cs) | Default stub |
| Agent Server | `0xB046` | [ActionTalkResponse](Disabled/Agent/Server/Action/ActionTalkResponse.cs) | Default stub |
| Agent Server | `0xB04B` | [ActionDeselectResponse](Disabled/Agent/Server/Action/ActionDeselectResponse.cs) | Default stub |
| Agent Server | `0xB04C` | [InventoryItemUseResponse](Agent/Server/Inventory/InventoryItemUseResponse.cs) | Preserved |
| Agent Server | `0xB050` | [CharacterIncreaseStrResponse](Disabled/Agent/Server/Character/CharacterIncreaseStrResponse.cs) | Default stub |
| Agent Server | `0xB051` | [CharacterIncreaseIntResponse](Disabled/Agent/Server/Character/CharacterIncreaseIntResponse.cs) | Default stub |
| Agent Server | `0xB067` | [PartyAutoRefuseResponse](Disabled/Agent/Server/Party/PartyAutoRefuseResponse.cs) | Default stub |
| Agent Server | `0xB069` | [PartyMatchingFormResponse](Disabled/Agent/Server/Party/PartyMatchingFormResponse.cs) | Default stub |
| Agent Server | `0xB06A` | [PartyMatchingChangeResponse](Disabled/Agent/Server/Party/PartyMatchingChangeResponse.cs) | Default stub |
| Agent Server | `0xB06B` | [PartyMatchingDeleteResponse](Disabled/Agent/Server/Party/PartyMatchingDeleteResponse.cs) | Default stub |
| Agent Server | `0xB06C` | [PartyMatchingListResponse](Disabled/Agent/Server/Party/PartyMatchingListResponse.cs) | Default stub |
| Agent Server | `0xB06D` | [PartyMatchingJoinResponse](Disabled/Agent/Server/Party/PartyMatchingJoinResponse.cs) | Default stub |
| Agent Server | `0xB070` | [ActionSkillCastResponse](Disabled/Agent/Server/Action/ActionSkillCastResponse.cs) | Default stub |
| Agent Server | `0xB071` | [ActionCommandResponse](Disabled/Agent/Server/Action/ActionCommandResponse.cs) | Default stub |
| Agent Server | `0xB072` | [ActionBuffRemoveResponse](Disabled/Agent/Server/Action/ActionBuffRemoveResponse.cs) | Default stub |
| Agent Server | `0xB074` | [ActionCommandStateResponse](Disabled/Agent/Server/Action/ActionCommandStateResponse.cs) | Default stub |
| Agent Server | `0xB081` | [ExchangeStartResponse](Agent/Server/Exchange/ExchangeStartResponse.cs) | Preserved |
| Agent Server | `0xB083` | [ExchangeClose](Agent/Server/Exchange/ExchangeClose.cs) | Preserved |
| Agent Server | `0xB084` | [ExchangeApprove](Agent/Server/Exchange/ExchangeApprove.cs) | Preserved |
| Agent Server | `0xB0A1` | [SkillLearnResponse](Disabled/Agent/Server/Skill/SkillLearnResponse.cs) | Default stub |
| Agent Server | `0xB0A2` | [SkillMasteryLearnResponse](Disabled/Agent/Server/Skill/SkillMasteryLearnResponse.cs) | Default stub |
| Agent Server | `0xB0A7` | [PlayerEnterBerzerkModeResponse](Disabled/Agent/Server/Game/PlayerEnterBerzerkModeResponse.cs) | Default stub |
| Agent Server | `0xB0BD` | [ActionBuffAddResponse](Disabled/Agent/Server/Action/ActionBuffAddResponse.cs) | Default stub |
| Agent Server | `0xB0C5` | [CosPickupAsyncResponse](Agent/Server/Cos/CosPickupAsyncResponse.cs) | Default stub |
| Agent Server | `0xB0C6` | [CosTerminateResponse](Agent/Server/Cos/CosTerminateResponse.cs) | Default stub |
| Agent Server | `0xB0CB` | [UpdateMountStateResponse](Agent/Server/Cos/UpdateMountStateResponse.cs) | Default stub |
| Agent Server | `0xB0D9` | [QuestAbandonResponse](Disabled/Agent/Server/Quest/QuestAbandonResponse.cs) | Default stub |
| Agent Server | `0xB0E1` | [JobJoinResponse](Disabled/Agent/Server/Job/JobJoinResponse.cs) | Default stub |
| Agent Server | `0xB0E2` | [JobLeaveResponse](Disabled/Agent/Server/Job/JobLeaveResponse.cs) | Default stub |
| Agent Server | `0xB0E3` | [JobAliasUpdateResponse](Disabled/Agent/Server/Job/JobAliasUpdateResponse.cs) | Default stub |
| Agent Server | `0xB150` | [ElixirAckResponseHandler](Disabled/Agent/Server/Alchemy/ElixirAckResponseHandler.cs) | Default stub |
| Agent Server | `0xB151` | [StoneAckResponseHandler](Disabled/Agent/Server/Alchemy/StoneAckResponseHandler.cs) | Default stub |
| Agent Server | `0xB202` | [SkillWithdrawResponse](Disabled/Agent/Server/Skill/SkillWithdrawResponse.cs) | Default stub |
| Agent Server | `0xB203` | [SkillMasteryWithdrawResponse](Disabled/Agent/Server/Skill/SkillMasteryWithdrawResponse.cs) | Default stub |
| Agent Server | `0xB250` | [ShoppingManagerOpenGuildStorageResponse](Disabled/Agent/Server/Guild/ShoppingManagerOpenGuildStorageResponse.cs) | Default stub |
| Agent Server | `0xB251` | [ShoppingManagerCloseGuildStorageResponse](Disabled/Agent/Server/Guild/ShoppingManagerCloseGuildStorageResponse.cs) | Default stub |
| Agent Server | `0xB504` | [LinkedItemResponseHandler](Disabled/Agent/Server/Game/LinkedItemResponseHandler.cs) | Default stub |
| Agent Server | `0xB516` | [EntityUpdatePvpFlag](Disabled/Agent/Server/Entity/EntityUpdatePvpFlag.cs) | Default stub |
| Agent Server | `0xB558` | [StorageBoxTakeItemResponse](Disabled/Agent/Server/StorageBox/StorageBoxTakeItemResponse.cs) | Default stub |
| Gateway Client | `0x2001` | [GlobalIdentificationRequest](Gateway/Client/Authentication/GlobalIdentificationRequest.cs) | Preserved |
| Gateway Client | `0x2002` | [KeepAliveRequest](Gateway/Client/Connection/KeepAliveRequest.cs) | Preserved |
| Gateway Client | `0x5000` | [HandshakeRequest](Gateway/Client/Connection/HandshakeRequest.cs) | Preserved |
| Gateway Client | `0x6100` | [PatchRequest](Gateway/Client/Authentication/PatchRequest.cs) | Preserved |
| Gateway Client | `0x6101` | [GatewayServerListRequestHook](Gateway/Client/Authentication/GatewayServerListRequestHook.cs) | Preserved |
| Gateway Client | `0x6102` | [GatewayLoginRequest](Gateway/Client/Authentication/GatewayLoginRequest.cs) | Preserved |
| Gateway Client | `0x6104` | [NoticeRequest](Gateway/Client/Game/NoticeRequest.cs) | Preserved |
| Gateway Client | `0x6106` | [ShardListPingRequest](Gateway/Client/Game/ShardListPingRequest.cs) | Preserved |
| Gateway Client | `0x610A` | [GatewayLoginRequestGlobal](Disabled/Gateway/Client/Authentication/GatewayLoginRequestGlobal.cs) | Default stub |
| Gateway Client | `0x610F` | [GatewayPendingCancelingRequest](Disabled/Gateway/Client/Game/GatewayPendingCancelingRequest.cs) | Default stub |
| Gateway Client | `0x6117` | [GlobalSecondaryPasswordRequest](Disabled/Gateway/Client/Authentication/GlobalSecondaryPasswordRequest.cs) | Default stub |
| Gateway Client | `0x6323` | [LoginIBUVAnswer](Disabled/Gateway/Client/Authentication/LoginIBUVAnswer.cs) | Preserved |
| Gateway Client | `0x9000` | [HandshakeAcceptRequest](Gateway/Client/Connection/HandshakeAcceptRequest.cs) | Preserved |
| Gateway Server | `0x2001` | [GlobalIdentificationRequest](Disabled/Gateway/Server/Authentication/GlobalIdentificationRequest.cs) | Default stub |
| Gateway Server | `0x210E` | [GatewayPendingUpdateResponse](Disabled/Gateway/Server/Game/GatewayPendingUpdateResponse.cs) | Default stub |
| Gateway Server | `0x2116` | [GlobalGatewayLoginAccepted](Disabled/Gateway/Server/Authentication/GlobalGatewayLoginAccepted.cs) | Default stub |
| Gateway Server | `0x2322` | [CaptchaDataResponse](Gateway/Server/Authentication/CaptchaDataResponse.cs) | Preserved |
| Gateway Server | `0x5000` | [HandshakeResponse](Gateway/Server/Connection/HandshakeResponse.cs) | Preserved |
| Gateway Server | `0x9000` | [HandshakeAcceptResponse](Disabled/Gateway/Server/Connection/HandshakeAcceptResponse.cs) | Default stub |
| Gateway Server | `0xA100` | [GatewayPatchResponse](Gateway/Server/Authentication/GatewayPatchResponse.cs) | Preserved |
| Gateway Server | `0xA101` | [GatewayServerListResponse](Gateway/Server/Authentication/GatewayServerListResponse.cs) | Preserved |
| Gateway Server | `0xA102` | [GatewayLoginResponse](Gateway/Server/Authentication/GatewayLoginResponse.cs) | Preserved |
| Gateway Server | `0xA10A` | [GatewayLoginResponseGlobal](Disabled/Gateway/Server/Authentication/GatewayLoginResponseGlobal.cs) | Default stub |
| Gateway Server | `0xA10F` | [GatewayPendingCancelingResponse](Disabled/Gateway/Server/Game/GatewayPendingCancelingResponse.cs) | Default stub |
| Gateway Server | `0xA117` | [GlobalSecondaryPasswordResponse](Disabled/Gateway/Server/Authentication/GlobalSecondaryPasswordResponse.cs) | Default stub |
| Gateway Server | `0xA323` | [CaptchaResponse](Disabled/Gateway/Server/Authentication/CaptchaResponse.cs) | Default stub |
