import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StandardService {
  constructor() {}
  standards: { control: string; statics: string[] }[] = [
    {
      control: 'toInsertFlagName',
      statics: ['insertflag', 'insertedflag', 'insert_flag', 'inserted_flag'],
    },
    {
      control: 'fromInsertFlagName',
      statics: ['insertflag', 'insertedflag', 'insert_flag', 'inserted_flag'],
    },
    {
      control: 'fromUpdateFlagName',
      statics: ['updateflag', 'updatedflag', 'updated_flag', 'update_flag'],
    },
    {
      control: 'toUpdateFlagName',
      statics: ['updateflag', 'updatedflag', 'updated_flag', 'update_flag'],
    },
    {
      control: 'ToDeleteFlagName',
      statics: ['deleteFlag', 'deleteflag', 'deleted_flag', 'delete_flag'],
    },
    {
      control: 'fromDeleteFlagName',

      statics: ['deleteFlag', 'deleteflag', 'deleted_flag', 'delete_flag'],
    },
    {
      control: 'localIdName',

      statics: ['local_id', 'localid'],
    },
    {
      control: 'cloudIdName',
      statics: ['cloud_id', 'cloudid'],
    },
  ];
}
