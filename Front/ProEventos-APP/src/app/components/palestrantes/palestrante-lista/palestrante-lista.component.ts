import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PalestranteService } from '@app/Services/palestrante.service';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { Palestrante } from '@app/models/Palestrante';
import { environment } from '@environments/environment';
import { BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-palestrante-lista',
  templateUrl: './palestrante-lista.component.html',
  styleUrls: ['./palestrante-lista.component.scss']
})
export class PalestranteListaComponent implements OnInit {
  public palestrantes: Palestrante[] = [];
  public PalestranteId: number;
  public pagination = {} as Pagination;

  termoBuscaChanged: Subject<string> = new Subject<string>(); // O termo de busca aqui é um subject


  constructor( private palestranteService: PalestranteService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router) { }

  ngOnInit() {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;

    this.carregarPalestrantes();
  }

  filtrarPalestrantes(evt: any): void // Quando eu digitar algo no campo do filtro ele vai ser chamado
  {if (this.termoBuscaChanged.observers.length === 0) { // Tem algo dentro do meu termo de busca ?
    this.termoBuscaChanged
    .pipe(debounceTime(1000)) // So vai fazer uma busca a cada 1 segundo aqui no caso
    .subscribe((filtrarPor) => {
      this.spinner.show();
      this.palestranteService
      .getPalestrantes(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        filtrarPor // O termo é retornado como parâmetro
        )
        .subscribe(
          (paginatedResult: PaginatedResult<Palestrante[]>) => {
            this.palestrantes = paginatedResult.result;
            this.pagination = paginatedResult.pagination;
          },
          (error: any) => {
            this.spinner.hide();
            this.toastr.error('Erro ao Carregar os Palestrantes', 'Erro!');
          }
          )
          .add(() => this.spinner.hide());
        });
      }
      this.termoBuscaChanged.next(evt.value); //
    }

    public getImageUrl(imagemName: string): string {
      if(imagemName)
        return environment.apiURL + `resources/perfil/${imagemName}`;
      else
        return './assets/img/perfil.png';
    }

    public carregarPalestrantes(): void {
      this.spinner.show();

      this.palestranteService.getPalestrantes(this.pagination.currentPage,
        this.pagination.itemsPerPage).subscribe(
          (paginatedResult: PaginatedResult<Palestrante[]>) => {
            this.palestrantes = paginatedResult.result;
            this.pagination = paginatedResult.pagination;
          },
          (error: any) => {
            this.spinner.hide();
            this.toastr.error('Erro ao Carregar os Palestrantes', 'Erro!');
          }
          )
          .add(() => this.spinner.hide());
        }
}
