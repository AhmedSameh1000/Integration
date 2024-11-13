import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListdbfromComponent } from './listdbfrom/listdbfrom.component';

const routes: Routes = [
  {
    path: '',
    component: ListdbfromComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FromDataBaseRoutingModule {}
