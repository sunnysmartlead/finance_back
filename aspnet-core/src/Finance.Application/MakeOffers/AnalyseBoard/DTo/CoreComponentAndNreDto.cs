using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Finance.PriceEval;
using Finance.PriceEval.Dto;

namespace Finance.MakeOffers.AnalyseBoard;

public class CoreComponentAndNreDto
{
    public virtual decimal TotalMoneyCynCount { get; set; }
    public virtual int year{ get; set; }
    
    
    /// <summary>
    /// 梯度
    /// </summary>
    [Required]
    public virtual List<GradientInput> Gradient { set; get; }

    public  List<string> projects{ set; get; }
    public  decimal kValue{ set; get; }
    public List<string> Costitems{ set; get; }
    public List<string> Nres{ set; get; }
    public List<Gradient> gradient{ set; get; }


}