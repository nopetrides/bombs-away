/*             |￣￣￣￣￣￣\                                                                  *
 *             |    STOP   |                                                                 *
 *             |   EDITING |                                                                 *
 *             |    THIS   |                                                                 *
 *             |    FILE   |                                                                 *
 *             | ＿＿＿＿＿ /                                                                 *
 *             (\__/)  ||                                                                    *
 *             (•ㅅ•)  ||                                                                    *
 *             / 　 づ                                                                       *
 *             ￣￣￣                                                                         *
 * This code was generated by our own generator! In order to modify this, please run the     *
 * generator with whatever settings you want.                                                *
 *                                                                                           */

using Murder.Components;
using Murder.Components.Agents;
using Murder.Component;
using Murder.Components.Cutscenes;
using Murder.Components.Serialization;
using Murder.Components.Effects;
using Bang.Interactions;
using Murder.Components.Graphics;
using Bang.StateMachines;
using Bang.Components;
using Murder.StateMachines;
using Murder.Interactions;
using HelloMurder.StateMachines;
using Murder.Messages;
using Murder.Core.Graphics;
using Murder.Messages.Physics;
using System.Collections.Immutable;

namespace Bang.Entities
{
    public enum HelloMurderComponentType
    {
        
    }

    public enum HelloMurderMessageType
    {
        
    }

    public static class HelloMurderEntityExtensions
    {
        #region Component "Get" methods!
        
        #endregion
        
        #region Component "Has" checkers!
        
        #endregion
        
        #region Component "TryGet" methods!
        
        #endregion
        
        #region Component "Set" methods!
        
        #endregion
        
        #region Component "Remove" methods!
        
        #endregion

        #region Message "Has" checkers!
        
        #endregion
    }

    public class HelloMurderLookupImplementation : MurderLookupImplementation
    {
        private static readonly ImmutableHashSet<int> _relativeComponents = new HashSet<int>()
        {
            86,
            118,
            118
        }.ToImmutableHashSet();

        public override ImmutableHashSet<int> RelativeComponents => _relativeComponents;

        private static readonly ImmutableDictionary<Type, int> _componentsIndex = new Dictionary<Type, int>()
        {
            { typeof(AdvancedCollisionComponent), 0 },
            { typeof(AgentComponent), 1 },
            { typeof(AgentImpulseComponent), 2 },
            { typeof(AgentSpeedMultiplier), 3 },
            { typeof(AgentSpeedOverride), 4 },
            { typeof(AgentSpriteComponent), 5 },
            { typeof(AlphaComponent), 6 },
            { typeof(AnimationCompleteComponent), 7 },
            { typeof(AnimationEventBroadcasterComponent), 8 },
            { typeof(AnimationOverloadComponent), 9 },
            { typeof(AnimationSpeedOverload), 10 },
            { typeof(AttackMultiplier), 11 },
            { typeof(BounceAmountComponent), 12 },
            { typeof(CameraFollowComponent), 13 },
            { typeof(CarveComponent), 14 },
            { typeof(ChoiceComponent), 15 },
            { typeof(ColliderComponent), 16 },
            { typeof(CollisionCacheComponent), 17 },
            { typeof(CustomCollisionMask), 18 },
            { typeof(CustomDrawComponent), 19 },
            { typeof(CustomTargetSpriteBatchComponent), 20 },
            { typeof(CutsceneAnchorsComponent), 21 },
            { typeof(CutsceneAnchorsEditorComponent), 22 },
            { typeof(DestroyAtTimeComponent), 23 },
            { typeof(DestroyOnAnimationCompleteComponent), 24 },
            { typeof(DestroyOnBlackboardConditionComponent), 25 },
            { typeof(DestroyOnCollisionComponent), 26 },
            { typeof(DisableAgentComponent), 27 },
            { typeof(DisableEntityComponent), 28 },
            { typeof(DisableParticleSystemComponent), 29 },
            { typeof(DisableSceneTransitionEffectsComponent), 30 },
            { typeof(DoNotPauseComponent), 31 },
            { typeof(DoNotPersistEntityOnSaveComponent), 32 },
            { typeof(DrawRectangleComponent), 33 },
            { typeof(EntityTrackerComponent), 34 },
            { typeof(EventListenerComponent), 35 },
            { typeof(EventListenerEditorComponent), 36 },
            { typeof(FacingComponent), 37 },
            { typeof(FadeScreenComponent), 38 },
            { typeof(FadeTransitionComponent), 39 },
            { typeof(FadeWhenInAreaComponent), 40 },
            { typeof(FadeWhenInCutsceneComponent), 41 },
            { typeof(FlashSpriteComponent), 42 },
            { typeof(FreeMovementComponent), 43 },
            { typeof(FreezeWorldComponent), 44 },
            { typeof(FrictionComponent), 45 },
            { typeof(GlobalShaderComponent), 46 },
            { typeof(GuidToIdTargetCollectionComponent), 47 },
            { typeof(GuidToIdTargetComponent), 48 },
            { typeof(HAAStarPathfindComponent), 49 },
            { typeof(HasVisionComponent), 50 },
            { typeof(HighlightOnChildrenComponent), 51 },
            { typeof(HighlightSpriteComponent), 52 },
            { typeof(IdTargetCollectionComponent), 53 },
            { typeof(IdTargetComponent), 54 },
            { typeof(IgnoreTriggersUntilComponent), 55 },
            { typeof(InCameraComponent), 56 },
            { typeof(IndestructibleComponent), 57 },
            { typeof(InsideMovementModAreaComponent), 58 },
            { typeof(InstanceToEntityLookupComponent), 59 },
            { typeof(InteractOnCollisionComponent), 60 },
            { typeof(InteractOnRuleMatchCollectionComponent), 61 },
            { typeof(InteractOnRuleMatchComponent), 62 },
            { typeof(InteractOnStartComponent), 63 },
            { typeof(InteractorComponent), 64 },
            { typeof(LineComponent), 65 },
            { typeof(MapComponent), 66 },
            { typeof(MovementModAreaComponent), 67 },
            { typeof(MoveToComponent), 68 },
            { typeof(MoveToPerfectComponent), 69 },
            { typeof(MusicComponent), 70 },
            { typeof(NineSliceComponent), 71 },
            { typeof(OnEnterOnExitComponent), 72 },
            { typeof(ParallaxComponent), 73 },
            { typeof(ParticleSystemComponent), 74 },
            { typeof(ParticleSystemWorldTrackerComponent), 75 },
            { typeof(PathfindComponent), 76 },
            { typeof(PauseAnimationComponent), 77 },
            { typeof(PickEntityToAddOnStartComponent), 78 },
            { typeof(PolygonSpriteComponent), 79 },
            { typeof(PrefabRefComponent), 80 },
            { typeof(PushAwayComponent), 81 },
            { typeof(QuadtreeComponent), 82 },
            { typeof(QuestTrackerComponent), 83 },
            { typeof(QuestTrackerRuntimeComponent), 84 },
            { typeof(RandomizeSpriteComponent), 85 },
            { typeof(RectPositionComponent), 86 },
            { typeof(ReflectionComponent), 87 },
            { typeof(RemoveColliderWhenStoppedComponent), 88 },
            { typeof(RemoveEntityOnRuleMatchAtLoadComponent), 89 },
            { typeof(RequiresVisionComponent), 90 },
            { typeof(RoomComponent), 91 },
            { typeof(RotateComponent), 92 },
            { typeof(RouteComponent), 93 },
            { typeof(RuleWatcherComponent), 94 },
            { typeof(ScaleComponent), 95 },
            { typeof(SituationComponent), 96 },
            { typeof(SoundComponent), 97 },
            { typeof(SoundParameterComponent), 98 },
            { typeof(SoundWatcherComponent), 99 },
            { typeof(SpeakerComponent), 100 },
            { typeof(SpriteComponent), 101 },
            { typeof(StateWatcherComponent), 102 },
            { typeof(StaticComponent), 103 },
            { typeof(StrafingComponent), 104 },
            { typeof(TextBoxComponent), 105 },
            { typeof(TextureComponent), 106 },
            { typeof(ThreeSliceComponent), 107 },
            { typeof(TileGridComponent), 108 },
            { typeof(TilesetComponent), 109 },
            { typeof(TintComponent), 110 },
            { typeof(UiDisplayComponent), 111 },
            { typeof(VelocityComponent), 112 },
            { typeof(VerticalPositionComponent), 113 },
            { typeof(WaitForVacancyComponent), 114 },
            { typeof(WindowRefreshTrackerComponent), 115 },
            { typeof(IStateMachineComponent), 116 },
            { typeof(IInteractiveComponent), 117 },
            { typeof(IMurderTransformComponent), 118 },
            { typeof(ITransformComponent), 118 },
            { typeof(StateMachineComponent<Coroutine>), 116 },
            { typeof(StateMachineComponent<DialogStateMachine>), 116 },
            { typeof(InteractiveComponent<AddChildOnInteraction>), 117 },
            { typeof(InteractiveComponent<AddComponentOnInteraction>), 117 },
            { typeof(InteractiveComponent<AddEntityOnInteraction>), 117 },
            { typeof(InteractiveComponent<AdvancedBlackboardInteraction>), 117 },
            { typeof(InteractiveComponent<BlackboardActionInteraction>), 117 },
            { typeof(InteractiveComponent<DebugInteraction>), 117 },
            { typeof(InteractiveComponent<EnableChildrenInteraction>), 117 },
            { typeof(InteractiveComponent<InteractChildOnInteraction>), 117 },
            { typeof(InteractiveComponent<InteractionCollection>), 117 },
            { typeof(InteractiveComponent<PlayMusicInteraction>), 117 },
            { typeof(InteractiveComponent<PlaySoundInteraction>), 117 },
            { typeof(InteractiveComponent<RemoveEntityOnInteraction>), 117 },
            { typeof(InteractiveComponent<SendToOtherInteraction>), 117 },
            { typeof(InteractiveComponent<SendToParentInteraction>), 117 },
            { typeof(InteractiveComponent<SetPositionInteraction>), 117 },
            { typeof(InteractiveComponent<SetSoundOnInteraction>), 117 },
            { typeof(InteractiveComponent<StopMusicInteraction>), 117 },
            { typeof(InteractiveComponent<TalkToInteraction>), 117 },
            { typeof(PositionComponent), 118 },
            { typeof(StateMachineComponent<MainMenuStateMachine>), 116 }
        }.ToImmutableDictionary();

        protected override ImmutableDictionary<Type, int> ComponentsIndex => _componentsIndex;

        private static readonly ImmutableDictionary<Type, int> _messagesIndex = new Dictionary<Type, int>()
        {
            { typeof(AnimationCompleteMessage), 119 },
            { typeof(AnimationEventMessage), 120 },
            { typeof(CollidedWithMessage), 121 },
            { typeof(FatalDamageMessage), 122 },
            { typeof(HighlightMessage), 123 },
            { typeof(InteractMessage), 124 },
            { typeof(InteractorMessage), 125 },
            { typeof(IsInsideOfMessage), 126 },
            { typeof(NextDialogMessage), 127 },
            { typeof(OnActorEnteredOrExitedMessage), 128 },
            { typeof(OnInteractExitMessage), 129 },
            { typeof(OnTriggerEnteredMessage), 130 },
            { typeof(PathNotPossibleMessage), 131 },
            { typeof(PickChoiceMessage), 132 },
            { typeof(TouchedGroundMessage), 133 }
        }.ToImmutableDictionary();

        protected override ImmutableDictionary<Type, int> MessagesIndex => _messagesIndex;
    }
}