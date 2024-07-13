import { NgModule } from '@angular/core';
import { BookStoreComponent } from './book-store/book-store.component';
import { SharedModule } from '../shared/shared.module';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { ReturnBookComponent } from './return-book/return-book.component';
import { AddbooksComponent } from './addbooks/addbooks.component';
import { EditbooksComponent } from './editbooks/editbooks.component';
import { BillingComponent } from './billing/billing.component';
import { BooksComponent } from './books/books.component';
import { FormsModule } from '@angular/forms';
import { DataTablesModule } from 'angular-datatables';

@NgModule({
  declarations: [BookStoreComponent, MaintenanceComponent, ReturnBookComponent, AddbooksComponent, EditbooksComponent, BillingComponent, BooksComponent],
  imports: [SharedModule, FormsModule, DataTablesModule],
})
export class BooksModule {}
