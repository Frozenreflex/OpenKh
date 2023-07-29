using Microsoft.Xna.Framework.Graphics;
using OpenKh.Common;
using OpenKh.Engine;
using OpenKh.Engine.MonoGame;
using OpenKh.Kh2;
using OpenKh.Tools.Kh2MsetEditorCrazyEdition.Helpers;
using OpenKh.Tools.Kh2MsetEditorCrazyEdition.Models.BoneDictSpec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xna = Microsoft.Xna.Framework;

namespace OpenKh.Tools.Kh2MsetEditorCrazyEdition.Usecases
{
    public class LoadModelUsecase
    {
        private readonly LoadMotionUsecase _loadMotionUsecase;
        private readonly LoadMotionDataUsecase _loadMotionDataUsecase;
        private readonly FilterBoneViewUsecase _filterBoneViewUsecase;
        private readonly BoneDictElement _boneDictElement;
        private readonly LoadedModel _loadedModel;

        public LoadModelUsecase(
            LoadedModel loadedModel,
            BoneDictElement boneDictElement,
            FilterBoneViewUsecase filterBoneViewUsecase,
            LoadMotionDataUsecase loadMotionDataUsecase,
            LoadMotionUsecase loadMotionUsecase
        )
        {
            _loadMotionUsecase = loadMotionUsecase;
            _loadMotionDataUsecase = loadMotionDataUsecase;
            _filterBoneViewUsecase = filterBoneViewUsecase;
            _boneDictElement = boneDictElement;
            _loadedModel = loadedModel;
        }

        public void OpenModel(string mdlxFile)
        {
            Close();

            _loadedModel.MdlxEntries = File.OpenRead(mdlxFile).Using(Bar.Read);
            _loadedModel.MdlxBytes = File.ReadAllBytes(mdlxFile);

            var barEntries = _loadedModel.MdlxEntries!;

            var model = Mdlx.Read(
                barEntries.First(it => it.Type == Bar.EntryType.Model).Stream
            );

            var textures = ModelTexture.Read(
                barEntries.First(it => it.Type == Bar.EntryType.ModelTexture).Stream
            )
                .Images;

            var modelMotionAttachable = MeshLoader.FromKH2(model);
            var tpose = modelMotionAttachable.MeshDescriptors;

            _loadedModel.ActiveFkBoneViews = _filterBoneViewUsecase
                .Filter(
                    _boneDictElement.BoneView,
                    new string[] { Path.GetFileNameWithoutExtension(mdlxFile) }
                );

            _loadedModel.FKJointDescriptions.Clear();
            _loadedModel.FKJointDescriptions.AddRange(
                ConvertBones(modelMotionAttachable.Bones)
            );
            _loadedModel.JointDescriptionsAge.Bump();

            _loadedModel.MdlxRenderableList.Add(
                new MdlxRenderSource(
                    model,
                    textures,
                    matrices =>
                    {
                        if (matrices != null)
                        {
                            modelMotionAttachable.ApplyMotion(matrices);
                            return modelMotionAttachable.MeshDescriptors;
                        }
                        else
                        {
                            return tpose;
                        }
                    }
                )
            );
        }

        private IEnumerable<JointDescription> ConvertBones(IReadOnlyList<Mdlx.Bone> bones)
        {
            int GetDepthOf(int index)
            {
                var depth = 0;
                while (true)
                {
                    var parent = bones[index].Parent;
                    if (parent < 0)
                    {
                        break;
                    }
                    else
                    {
                        index = parent;
                        ++depth;
                    }
                }
                return depth;
            }

            return bones
                .Select(
                    (bone, index) => new JointDescription(index, GetDepthOf(index))
                );
        }

        public void Close()
        {
            _loadedModel.MdlxRenderableList.Clear();

            _loadMotionUsecase.Close();
            _loadMotionDataUsecase.Close();
        }
    }
}
