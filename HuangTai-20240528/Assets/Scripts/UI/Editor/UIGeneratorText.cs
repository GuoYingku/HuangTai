using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIGenerator
{
    private const string ADDRESSABLE_SETTING_PATH = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    private const string VIEW_PREFAB = "Assets/Res/UI/ViewPrefab.prefab";

    private static readonly string SCRIPT_PARENT_DIR = Application.dataPath + "/Scripts/UI/UI";
    private static readonly string RES_PARENT_DIR = Application.dataPath + "/Res/UI/UI";

    private static readonly string UIID_PATH = Application.dataPath + "/Scripts/UI/UIID.cs";

    private static readonly string SCRIPT_DIR_TEMPLATE = SCRIPT_PARENT_DIR + "/{0}";
    private static readonly string RES_DIR_TEMPLATE = RES_PARENT_DIR + "/{0}";

    private static readonly string IVIEW_PATH_TEMPLATE = SCRIPT_DIR_TEMPLATE + "/I{0}View.cs";
    private static readonly string IPRESENTER_PATH_TEMPLATE = SCRIPT_DIR_TEMPLATE + "/I{0}Presenter.cs";
    private static readonly string VIEW_PATH_TEMPLATE = SCRIPT_DIR_TEMPLATE + "/{0}View.cs";
    private static readonly string PRESENTER_PATH_TEMPLATE = SCRIPT_DIR_TEMPLATE + "/{0}Presenter.cs";

    private static readonly string VIEW_PREFAB_PATH_TEMPLATE = "Assets/Res/UI/UI/{0}/{0}View.prefab";

    private static readonly string IVIEW_TEMPLATE =
@"public interface I{0}View : IUIView
{{
}}";
    private static readonly string IPRESENTER_TEMPLATE =
@"public interface I{0}Presenter : IUIPresenter
{{
}}";
    private static readonly string VIEW_TEMPLATE =
@"[CanvasIndex(0)]
public class {0}View : UIView<{0}Presenter>, I{0}View
{{
    public override void InitUIElements()
    {{
    }}
}}";
    private static readonly string PRESENTER_TEMPLATE =
@"public class {0}Presenter : UIPresenter<{0}View>, I{0}Presenter
{{
}}";
    private static readonly string UIID_TEMPLATE =
@"//��UIGenerator�Զ����ɣ������޸���ȷ��������ȷ��
public partial class UIID
{{
{0}
}}";
    private static readonly string UIID_LINE_TEMPLATE =
@"    public static readonly UIID {0} = new UIID(""{0}"",typeof({0}View),typeof({0}Presenter));";


    private static readonly string EXISTING_WARNING_TEMPLATE = "{0}View�����Ѵ��ڣ���ȷ�ϡ�";
}