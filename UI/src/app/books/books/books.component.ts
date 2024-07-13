import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/services/api.service';
import { BooksWithPagination } from '../../models/models';
import { Config } from 'datatables.net';
import { Subject } from 'rxjs';
import * as DataTables from 'datatables.net';


@Component({
  selector: 'books',
  templateUrl: './books.component.html',
  styleUrl: './books.component.scss'
})
export class BooksComponent  implements OnInit {
  books: BooksWithPagination[] = [];
  searchTerm: string = '';
  pageNumber: number = 1;
  pageSize: number = 100;
  // dtOptions: DataTables.Settings = {};
  dtOptions: Config = {};
  dttrigger: Subject<any>=new Subject<any>();

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.getBooks();
    this.dtOptions={
      pagingType: 'full_numbers',
      pageLength: 10,
      processing: true
    }
  }

  getBooks(): void {
    this.apiService.getBooksWithPagination(this.searchTerm, this.pageNumber, this.pageSize)
      .subscribe(books => {
        this.books = books;
        this.dttrigger.next(null);
      });
  }

  search(): void {
    this.pageNumber = 1; // Reset page number when searching
    this.getBooks();
  }

  prevPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.getBooks();
    }
  }

  nextPage(): void {
    this.pageNumber++;
    this.getBooks();
  }

  deleteBook(id: number): void {
    if (confirm('Are you sure you want to delete this book?')) {
      this.apiService.deleteBook(id).subscribe(
        () => {
          alert('Book deleted successfully');
          // this.refreshTable();
        },
        error => {
          console.error('Error deleting book:', error);
          alert('Failed to delete book');
        }
      );
    }
  }

  // refreshTable(): void {
  //   // Re-fetch the updated list of books
  //   this.getBooks();
  // }
}
