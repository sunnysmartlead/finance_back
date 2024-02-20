
using Abp.UI;
using AutoMapper;
using Finance.Dto;
using Finance.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.ProductDevelopment.Dto
{
    public class StructureBomDto
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }
        /// <summary>
        /// 超级大种类
        /// </summary>
        public string SuperTypeName { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 是否涉及（必填）
        /// </summary>
        public string IsInvolveItem { get; set; }
        /// <summary>
        /// 物料编号（SAP）
        /// </summary>
        public string SapItemNum { get; set; }
        /// <summary>
        /// 图号名称
        /// </summary>
        public string DrawingNumName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// 外形尺寸mm
        /// </summary>
        public string OverallDimensionSize { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 重量g
        /// </summary>
        public double WeightNumber { get; set; }
        /// <summary>
        /// 成型工艺
        /// </summary>
        public string MoldingProcess { get; set; }
        /// <summary>
        /// 是否新开模
        /// </summary>
        public string IsNewMouldProduct { get; set; }
        /// <summary>
        /// 二次加工方法
        /// </summary>
        public string SecondaryProcessingMethod { get; set; }
        /// <summary>
        /// 表面处理
        /// </summary>
        public string SurfaceTreatmentMethod { get; set; }
        /// <summary>
        /// 关键尺寸精度及重要要求
        /// </summary>
        public string DimensionalAccuracyRemark { get; set; }

        /// <summary>
        /// 电子BOM文件Id
        /// </summary>
        public long FileId { get; set; }
    }


    public class ElectronicBomDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 是否涉及（必填）
        /// </summary>
        public string IsInvolveItem { get; set; }
        /// <summary>
        /// 物料编号（SAP）
        /// </summary>
        public string SapItemNum { get; set; }
        /// <summary>
        /// 材料名称
        public string SapItemName { get; set; }
        /// <summary>
        /// 装配数量
        /// </summary>
        public double AssemblyQuantity { get; set; }
        /// <summary>
        /// 封装(需要体现PAD的数量)
        /// </summary>
        public string EncapsulationSize { get; set; }

        /// <summary>
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        /// <summary>
        public string Product { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }

        /// <summary>
        /// 电子BOM文件Id
        /// </summary>
        public long FileId { get; set; }
    }


    public class BoardDto
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        //[FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        /// <summary>
        public string Product { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        //[FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }

        /// <summary>
        /// 板部件序号
        /// </summary>
        public long BoardId { get; set; }
        /// <summary>
        /// 板部件名称
        /// </summary>
        [FriendlyRequired("板部件名称", SpecialVerification.Nothing)]
        public string BoardName { get; set; }
        /// <summary>
        /// 板部件长
        /// </summary>
       [FriendlyRequired("板部件长", SpecialVerification.Nothing)]
        public decimal BoardLenth { get; set; }
        /// <summary>
        /// 板部件宽
        /// </summary>
        [FriendlyRequired("板部件宽", SpecialVerification.Nothing)]
        public decimal BoardWidth { get; set; }
        /// <summary>
        /// 板部件面积
        /// </summary>

        public decimal BoardSquare { get; set; }
        /// <summary>
        /// 拼版数量
        /// </summary>
        public long StoneQuantity { get; set; }

    }

    public class SERow
    {
        /// <summary>
        /// 开始行
        /// </summary>
        public int StartLine { get; set; }
        /// <summary>
        /// 结束行
        /// </summary>
        public int EndLine { get; set; }
        /// <summary>
        /// 第几个物料大类
        /// </summary>
        public int WhichMaterialCategory { get; set; }
    }
    public class KeyOrValue
    {      
        public string Key { get; set; }    
        public string Value { get; set; }        
    }
    public class ProductDevelopmentInputDto: ToExamineDto
    {
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }
        /// <summary>
        /// 3D爆炸图Id
        /// </summary>
        public long Picture3DFileId { get; set; }
        /// <summary>
        /// 像素M
        /// </summary>
        public string Pixel { get; set; }
        /// <summary>
        /// FOV
        /// </summary>
        public string FOV { get; set; }

        /// <summary>
        /// 外包装长
        /// </summary>
        public string OuterPackagingLength { get; set; }
        /// <summary>
        /// 外包装宽
        /// </summary>
        public string OuterPackagingWidth { get; set; }
        /// <summary>
        /// 外包装高
        /// </summary>
        public string OuterPackagingHeight { get; set; }
        /// <summary>
        /// 单个产品重量
        /// </summary>
        public string SingleProductWeight { get; set; }
        /// <summary>
        /// 单箱数量
        /// </summary>
        public string SingleBoxQuantity { get; set; }
        /// <summary>
        /// 内包装长
        /// </summary>
        public string InnerPackagingLength { get; set; }
        /// <summary>
        /// 内包装宽
        /// </summary>
        public string InnerPackagingWidth { get; set; }
        /// <summary>
        /// 内包装高
        /// </summary>
        public string InnerPackagingHeight { get; set; }
        /// <summary>
        /// 是否打托
        /// </summary>
        public string IsHit { get; set; }
        /// <summary>
        /// 每托盘箱数
        /// </summary>
        public string BoxesPerPallet { get; set; }
        /// <summary>
        /// 单箱包装数量
        /// </summary>
        public string QuantityPerBox { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 客户特殊性需求
        /// </summary>
        public string Requirement { get; set; }
        /// <summary>
        /// SOR附件Id
        /// </summary>
        public string SorId { get; set; }
        /// <summary>
        /// 结构BOMlist
        /// </summary>
        public List<StructureBomDto> StructureBomDtos { get; set; }
        /// <summary>
        /// 电子BOMlist
        /// </summary>
        public List<ElectronicBomDto> ElectronicBomDtos { get; set; }
        /// <summary>
        /// 拼版信息list
        /// </summary>
        public List<BoardDto> BoardDtos { get; set; }

        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 运输方式（字典明细表主键）
        /// </summary>
        public virtual string ShippingType { get; set; }

        /// <summary>
        /// 包装方式（字典明细表主键）
        /// </summary>
        public virtual string PackagingType { get; set; }

        /// <summary>
        /// 交货地点
        /// </summary>
        public virtual string PlaceOfDelivery { get; set; }

        /// <summary>
        /// 电子BOM文件Id
        /// </summary>
        public long ElcFileId { get; set; }
        /// <summary>
        /// 结构BOM文件Id
        /// </summary>
        public long StuFileId { get; set; }

    }



    public class PostProductDevelopmentInputDto
    {
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1、零件2...）
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }
        /// <summary>
        /// 3D爆炸图Id
        /// </summary>
        public string Picture3DFileId { get; set; }
        /// <summary>
        /// 像素M
        /// </summary>
        public string Pixel { get; set; }
        /// <summary>
        /// FOV
        /// </summary>
        public string FOV { get; set; }

        /// <summary>
        /// 外包装长
        /// </summary>
        public string OuterPackagingLength { get; set; }
        /// <summary>
        /// 外包装宽
        /// </summary>
        public string OuterPackagingWidth { get; set; }
        /// <summary>
        /// 外包装高
        /// </summary>
        public string OuterPackagingHeight { get; set; }
        /// <summary>
        /// 单个产品重量
        /// </summary>
        public string SingleProductWeight { get; set; }
        /// <summary>
        /// 单箱数量
        /// </summary>
        public string SingleBoxQuantity { get; set; }
        /// <summary>
        /// 内包装长
        /// </summary>
        public string InnerPackagingLength { get; set; }
        /// <summary>
        /// 内包装宽
        /// </summary>
        public string InnerPackagingWidth { get; set; }
        /// <summary>
        /// 内包装高
        /// </summary>
        public string InnerPackagingHeight { get; set; }
        /// <summary>
        /// 是否打托
        /// </summary>
        public string IsHit { get; set; }
        /// <summary>
        /// 每托盘箱数
        /// </summary>
        public string BoxesPerPallet { get; set; }
        /// <summary>
        /// 单箱包装数量
        /// </summary>
        public string QuantityPerBox { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 客户特殊性需求
        /// </summary>
        public string Requirement { get; set; }
        /// <summary>
        /// SOR附件Id
        /// </summary>
        public string SorId { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 运输方式（字典明细表主键）
        /// </summary>
        public virtual string ShippingType { get; set; }

        /// <summary>
        /// 包装方式（字典明细表主键）
        /// </summary>
        public virtual string PackagingType { get; set; }

        /// <summary>
        /// 交货地点
        /// </summary>
        public virtual string PlaceOfDelivery { get; set; }



    }


    /// <summary>
    /// 自定义mapper映射并对数据类型更改
    /// </summary>
    public class ProductDevelopmentInputDtoConverter : ITypeConverter<ProductDevelopmentInputDto, ProductDevelopmentInput>
    {

        public ProductDevelopmentInput Convert(ProductDevelopmentInputDto dto, ProductDevelopmentInput destination, ResolutionContext context)
        {
           
            try {
                ProductDevelopmentInput dest = new ProductDevelopmentInput
                {
                    AuditFlowId = dto.AuditFlowId,
                    ProductId = dto.ProductId,
                    Product=dto.Product,
                    SolutionId = dto.SolutionId,
                    SolutionNum=dto.SolutionNum,
                    Picture3DFileId = System.Convert.ToInt64(dto.Picture3DFileId),
                    OuterPackagingLength = System.Convert.ToDouble(dto.OuterPackagingLength),
                    OuterPackagingWidth = System.Convert.ToDouble(dto.OuterPackagingWidth),
                    OuterPackagingHeight = System.Convert.ToDouble(dto.OuterPackagingHeight),
                    SingleProductWeight = System.Convert.ToDouble(dto.SingleProductWeight),
                    SingleBoxQuantity = System.Convert.ToInt32(dto.SingleBoxQuantity),
                    InnerPackagingLength = System.Convert.ToDouble(dto.InnerPackagingLength),
                    InnerPackagingWidth = System.Convert.ToDouble(dto.InnerPackagingWidth),
                    InnerPackagingHeight = System.Convert.ToDouble(dto.InnerPackagingHeight),
                    IsHit = dto.IsHit,
                    BoxesPerPallet = System.Convert.ToInt32(dto.BoxesPerPallet),
                    QuantityPerBox = System.Convert.ToInt32(dto.QuantityPerBox),
                    Remarks = dto.Remarks
                };

                return dest;
            } catch {
                throw new UserFriendlyException("类型转换错误，请判断输入在数值是否有误");
            }
            return null;

           
        }
    }

}
