/*
	Generated by KBEngine!
	Please do not modify this file!
	
	tools = kbcmd
*/

namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	// defined in */scripts/entity_defs/Avatar.def
	public class EntityBaseEntityCall_AvatarBase : EntityCall
	{
		public EntityBaseEntityCall_TestBase component1 = null;
		public EntityBaseEntityCall_TestBase component2 = null;
		public EntityBaseEntityCall_TestNoBaseBase component3 = null;

		public EntityBaseEntityCall_AvatarBase(Int32 eid, string ename) : base(eid, ename)
		{
			component1 = new EntityBaseEntityCall_TestBase(16, id);
			component2 = new EntityBaseEntityCall_TestBase(21, id);
			component3 = new EntityBaseEntityCall_TestNoBaseBase(22, id);
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_BASE;
		}

	}

	public class EntityCellEntityCall_AvatarBase : EntityCall
	{
		public EntityCellEntityCall_TestBase component1 = null;
		public EntityCellEntityCall_TestBase component2 = null;
		public EntityCellEntityCall_TestNoBaseBase component3 = null;

		public EntityCellEntityCall_AvatarBase(Int32 eid, string ename) : base(eid, ename)
		{
			component1 = new EntityCellEntityCall_TestBase(16, id);
			component2 = new EntityCellEntityCall_TestBase(21, id);
			component3 = new EntityCellEntityCall_TestNoBaseBase(22, id);
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL;
		}

		public void dialog(Int32 arg1, UInt32 arg2)
		{
			Bundle pBundle = newCall("dialog", 0);
			if(pBundle == null)
				return;

			bundle.writeInt32(arg1);
			bundle.writeUint32(arg2);
			sendCall(null);
		}

		public void jump()
		{
			Bundle pBundle = newCall("jump", 0);
			if(pBundle == null)
				return;

			sendCall(null);
		}

		public void relive(Byte arg1)
		{
			Bundle pBundle = newCall("relive", 0);
			if(pBundle == null)
				return;

			bundle.writeUint8(arg1);
			sendCall(null);
		}

		public void requestPull()
		{
			Bundle pBundle = newCall("requestPull", 0);
			if(pBundle == null)
				return;

			sendCall(null);
		}

		public void useTargetSkill(Int32 arg1, Int32 arg2)
		{
            Write.Log("cellCall useTargetSkill");
			Bundle pBundle = newCall("useTargetSkill", 0);
			if(pBundle == null)
				return;

			bundle.writeInt32(arg1);
			bundle.writeInt32(arg2);
			sendCall(null);
		}

	}
	}
