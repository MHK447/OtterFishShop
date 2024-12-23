// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace BanpoFri.Data
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct UserData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static UserData GetRootAsUserData(ByteBuffer _bb) { return GetRootAsUserData(_bb, new UserData()); }
  public static UserData GetRootAsUserData(ByteBuffer _bb, UserData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public UserData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Money { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMoneyBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetMoneyBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetMoneyArray() { return __p.__vector_as_array<byte>(4); }
  public int Cash { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateCash(int cash) { int o = __p.__offset(6); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, cash); return true; } else { return false; } }
  public long Lastlogintime { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetLong(o + __p.bb_pos) : (long)0; } }
  public bool MutateLastlogintime(long lastlogintime) { int o = __p.__offset(8); if (o != 0) { __p.bb.PutLong(o + __p.bb_pos, lastlogintime); return true; } else { return false; } }
  public long Curplaydatetime { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetLong(o + __p.bb_pos) : (long)0; } }
  public bool MutateCurplaydatetime(long curplaydatetime) { int o = __p.__offset(10); if (o != 0) { __p.bb.PutLong(o + __p.bb_pos, curplaydatetime); return true; } else { return false; } }
  public BanpoFri.Data.StageData? Stagedata { get { int o = __p.__offset(12); return o != 0 ? (BanpoFri.Data.StageData?)(new BanpoFri.Data.StageData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public BanpoFri.Data.RecordCount? Recordcount(int j) { int o = __p.__offset(14); return o != 0 ? (BanpoFri.Data.RecordCount?)(new BanpoFri.Data.RecordCount()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int RecordcountLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }
  public BanpoFri.Data.UpgradeData? Upgradedatas(int j) { int o = __p.__offset(16); return o != 0 ? (BanpoFri.Data.UpgradeData?)(new BanpoFri.Data.UpgradeData()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int UpgradedatasLength { get { int o = __p.__offset(16); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<BanpoFri.Data.UserData> CreateUserData(FlatBufferBuilder builder,
      StringOffset moneyOffset = default(StringOffset),
      int cash = 0,
      long lastlogintime = 0,
      long curplaydatetime = 0,
      Offset<BanpoFri.Data.StageData> stagedataOffset = default(Offset<BanpoFri.Data.StageData>),
      VectorOffset recordcountOffset = default(VectorOffset),
      VectorOffset upgradedatasOffset = default(VectorOffset)) {
    builder.StartTable(7);
    UserData.AddCurplaydatetime(builder, curplaydatetime);
    UserData.AddLastlogintime(builder, lastlogintime);
    UserData.AddUpgradedatas(builder, upgradedatasOffset);
    UserData.AddRecordcount(builder, recordcountOffset);
    UserData.AddStagedata(builder, stagedataOffset);
    UserData.AddCash(builder, cash);
    UserData.AddMoney(builder, moneyOffset);
    return UserData.EndUserData(builder);
  }

  public static void StartUserData(FlatBufferBuilder builder) { builder.StartTable(7); }
  public static void AddMoney(FlatBufferBuilder builder, StringOffset moneyOffset) { builder.AddOffset(0, moneyOffset.Value, 0); }
  public static void AddCash(FlatBufferBuilder builder, int cash) { builder.AddInt(1, cash, 0); }
  public static void AddLastlogintime(FlatBufferBuilder builder, long lastlogintime) { builder.AddLong(2, lastlogintime, 0); }
  public static void AddCurplaydatetime(FlatBufferBuilder builder, long curplaydatetime) { builder.AddLong(3, curplaydatetime, 0); }
  public static void AddStagedata(FlatBufferBuilder builder, Offset<BanpoFri.Data.StageData> stagedataOffset) { builder.AddOffset(4, stagedataOffset.Value, 0); }
  public static void AddRecordcount(FlatBufferBuilder builder, VectorOffset recordcountOffset) { builder.AddOffset(5, recordcountOffset.Value, 0); }
  public static VectorOffset CreateRecordcountVector(FlatBufferBuilder builder, Offset<BanpoFri.Data.RecordCount>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateRecordcountVectorBlock(FlatBufferBuilder builder, Offset<BanpoFri.Data.RecordCount>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartRecordcountVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUpgradedatas(FlatBufferBuilder builder, VectorOffset upgradedatasOffset) { builder.AddOffset(6, upgradedatasOffset.Value, 0); }
  public static VectorOffset CreateUpgradedatasVector(FlatBufferBuilder builder, Offset<BanpoFri.Data.UpgradeData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateUpgradedatasVectorBlock(FlatBufferBuilder builder, Offset<BanpoFri.Data.UpgradeData>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartUpgradedatasVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<BanpoFri.Data.UserData> EndUserData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<BanpoFri.Data.UserData>(o);
  }
  public static void FinishUserDataBuffer(FlatBufferBuilder builder, Offset<BanpoFri.Data.UserData> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedUserDataBuffer(FlatBufferBuilder builder, Offset<BanpoFri.Data.UserData> offset) { builder.FinishSizePrefixed(offset.Value); }
};


}
