import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatabaseRoutingModule } from './database-routing.module';
import { ListdatabasesComponent } from './listdatabases/listdatabases.component';
import { SharedModule } from '../shared/shared.module';
import { MangeDataBaseComponent } from './mange-data-base/mange-data-base.component';
import { MangeDataBaseColumnsComponent } from './mange-data-base-columns/mange-data-base-columns.component';

@NgModule({
  declarations: [ListdatabasesComponent, MangeDataBaseComponent, MangeDataBaseColumnsComponent],
  imports: [CommonModule, DatabaseRoutingModule, SharedModule],
})
export class DatabaseModule {}
