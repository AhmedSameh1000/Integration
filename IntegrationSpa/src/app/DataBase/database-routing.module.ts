import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListdatabasesComponent } from './listdatabases/listdatabases.component';

const routes: Routes = [
  {
    path: 'databases',
    component: ListdatabasesComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DatabaseRoutingModule {}
