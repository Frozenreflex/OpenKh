using OpenKh.Engine.MonoGame;
using OpenKh.Kh2;
using OpenKh.Tools.Kh2MsetEditorCrazyEdition.Models.BoneDictSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static OpenKh.Kh2.Motion;

namespace OpenKh.Tools.Kh2MsetEditorCrazyEdition.Helpers
{
    public class LoadedModel
    {
        public Bar? MdlxEntries { get; set; }
        public Bar? MsetEntries { get; set; }
        public Bar? AnbEntries { get; set; }
        public List<MdlxRenderSource> MdlxRenderableList { get; set; } = new List<MdlxRenderSource>();
        public List<MotionDisplay> MotionList { get; set; } = new List<MotionDisplay>();
        public int SelectedMotionIndex { get; set; } = -1;
        public InterpolatedMotion? MotionData { get; set; }
        public float FrameTime { get; set; }
        public Func<float, Matrix4x4[]>? PoseProvider { get; set; }

        /// <summary>
        /// Needed for Kh2AnimEmu
        /// </summary>
        public byte[]? MdlxBytes { get; set; }

        public Dictionary<ModelTexture.Texture, KingdomTexture> KingdomTextureCache { get; set; } = new Dictionary<ModelTexture.Texture, KingdomTexture>();

        /// <summary>
        /// From anb
        /// </summary>
        public float FramePerSecond { get; set; } = 0;
        /// <summary>
        /// From anb
        /// </summary>
        public float FrameEnd { get; set; } = 0;

        public OneTimeOn OpenMotionPlayerOnce { get; set; } = new OneTimeOn(false);

        public List<JointDescription> FKJointDescriptions { get; set; } = new List<JointDescription>();
        public List<JointDescription> IKJointDescriptions { get; set; } = new List<JointDescription>();
        public AgeManager JointDescriptionsAge { get; set; } = new AgeManager();

        public IEnumerable<BoneElement>? ActiveFkBoneViews { get; set; }
        public IEnumerable<BoneElement>? ActiveIkBoneViews { get; set; }

        public int SelectedJointIndex { get; set; } = -1;
        public AgeManager SelectedJointIndexAge { get; set; } = new AgeManager();

        public Dictionary<Bar.Entry, InterpolatedMotion> InterpolatedMotionCache { get; set; } = new Dictionary<Bar.Entry, InterpolatedMotion>();

        /// <summary>
        /// Bump if MotionData has been modified
        /// </summary>
        public AgeManager MotionDataAge { get; set; } = new AgeManager();
    }
}
