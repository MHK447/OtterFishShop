// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace BanpoFri.Data
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct SkillCardData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static SkillCardData GetRootAsSkillCardData(ByteBuffer _bb) { return GetRootAsSkillCardData(_bb, new SkillCardData()); }
  public static SkillCardData GetRootAsSkillCardData(ByteBuffer _bb, SkillCardData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public SkillCardData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Skillidx { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateSkillidx(int skillidx) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, skillidx); return true; } else { return false; } }
  public int Level { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateLevel(int level) { int o = __p.__offset(6); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, level); return true; } else { return false; } }

  public static Offset<BanpoFri.Data.SkillCardData> CreateSkillCardData(FlatBufferBuilder builder,
      int skillidx = 0,
      int level = 0) {
    builder.StartTable(2);
    SkillCardData.AddLevel(builder, level);
    SkillCardData.AddSkillidx(builder, skillidx);
    return SkillCardData.EndSkillCardData(builder);
  }

  public static void StartSkillCardData(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddSkillidx(FlatBufferBuilder builder, int skillidx) { builder.AddInt(0, skillidx, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static Offset<BanpoFri.Data.SkillCardData> EndSkillCardData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<BanpoFri.Data.SkillCardData>(o);
  }
};


}
