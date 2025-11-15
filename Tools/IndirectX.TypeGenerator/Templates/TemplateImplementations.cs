using System.Collections.Generic;
using System.Linq;

namespace IndirectX.TypeGenerator.Templates;

public partial class EnumTemplate
{
    protected IEnumerable<EnumModel> Model;
    protected TypeGeneratorSetting Setting;

    public EnumTemplate(IEnumerable<EnumDefinition> model, TypeGeneratorSetting setting)
    {
        Model = model.Select(m => new EnumModel(setting, m));
        Setting = setting;
    }
}

public partial class StructTemplate
{
    protected IEnumerable<StructModel> Model;
    protected TypeGeneratorSetting Setting;

    public StructTemplate(IEnumerable<StructDefinition> model, TypeGeneratorSetting setting)
    {
        Model = model.Select(m => new StructModel(setting, m));
        Setting = setting;
    }
}

public partial class InterfaceTemplate
{
    protected InterfaceModel Model;
    protected TypeGeneratorSetting Setting;

    public InterfaceTemplate(InterfaceDefinition model, TypeGeneratorSetting setting)
    {
        Model = new InterfaceModel(setting, model);
        Setting = setting;
    }
}

public partial class ApiTemplate
{
    protected IEnumerable<MethodModel> Model;
    protected TypeGeneratorSetting Setting;

    public ApiTemplate(IEnumerable<MethodDefinition> model, TypeGeneratorSetting setting)
    {
        Model = model.Select(m => new MethodModel(setting, null, m));
        Setting = setting;
    }
}
