using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utility.EditorTools
{
    public class MoveByOneUnit
    {
        [MenuItem("Tools/MoveByOneUnit/MoveLeftByOneUnit #&%a")]
        public static void MoveLeftMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Left");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(-1, 0, 0);
            }
        }
        [MenuItem("Tools/MoveByOneUnit/MoveRightByOneUnit #&%d")]
        public static void MoveRightMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Right");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(1, 0, 0);
            }
        }
        [MenuItem("Tools/MoveByOneUnit/MoveForwardByOneUnit #&%w")]
        public static void MoveForwardMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Forward");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(0, 0, 1);
            }
        }
        [MenuItem("Tools/MoveByOneUnit/MoveBackwardByOneUnit #&%s")]
        public static void MoveBackwardMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Backward");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(0, 0, -1);
            }
        }
        [MenuItem("Tools/MoveByOneUnit/MoveUpByOneUnit #&%q")]
        public static void MoveUpMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Up");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(0, -1, 0);
            }
        }
        [MenuItem("Tools/MoveByOneUnit/MoveDownByOneUnit #&%e")]
        public static void MoveDownMenuItem()
        {
            var objects = Selection.GetTransforms(SelectionMode.TopLevel);
            Undo.RecordObjects(objects, "Move Down");
            foreach (var t in objects)
            {
                t.transform.ChangeLocalPosition(0, 1, 0);
            }
        }
    }
}