import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatabaseRoutingModule } from './database-routing.module';
import { ListdatabasesComponent } from './listdatabases/listdatabases.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [ListdatabasesComponent],
  imports: [CommonModule, DatabaseRoutingModule, SharedModule],
})
export class DatabaseModule {}
