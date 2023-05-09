import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/Services/evento.service';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef = {} as BsModalRef;
  public eventos: Evento[] = [];
  public eventoId: number;
  public pagination = {} as Pagination;

  public widthImg = 150;
  public marginImg = 2;
  public showImg = true;

  termoBuscaChanged: Subject<string> = new Subject<string>(); // O termo de busca aqui é um subject

  filtrarEventos(evt: any): void // Quando eu digitar algo no campo do filtro ele vai ser chamado
  {if (this.termoBuscaChanged.observers.length === 0) { // Tem algo dentro do meu termo de busca ?
    this.termoBuscaChanged
    .pipe(debounceTime(1000)) // So vai fazer uma busca a cada 1 segundo aqui no caso
    .subscribe((filtrarPor) => {
      this.spinner.show();
      this.eventoService
      .getEventos(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        filtrarPor // O termo é retornado como parâmetro
        )
        .subscribe(
          (paginatedResult: PaginatedResult<Evento[]>) => {
            this.eventos = paginatedResult.result;
            this.pagination = paginatedResult.pagination;
          },
          (error: any) => {
            this.spinner.hide();
            this.toastr.error('Erro ao Carregar os Eventos', 'Erro!');
          }
          )
          .add(() => this.spinner.hide());
        });
      }
      this.termoBuscaChanged.next(evt.value); //
    }

    constructor(
      private eventoService: EventoService,
      private modalService: BsModalService,
      private toastr: ToastrService,
      private spinner: NgxSpinnerService,
      private router: Router
      ) { }

      ngOnInit(): void {
        this.pagination = {
          currentPage: 1,
          itemsPerPage: 3,
          totalItems: 1,
        } as Pagination;

        this.carregarEventos();

        setTimeout(() => {
          /** spinner ends after 5 seconds */
          this.spinner.hide();
        }, 5000);
      }

      alterarImagem(): void{
        this.showImg = !this.showImg; // vai alterar o estado da imagem caso clique no botão
      }

      mostrarImagem(imageURL: string): string {
        return (imageURL !== '')
        ? `${environment.apiURL}resources/images/${imageURL}`
        : 'assets/semImagem.jpeg';
      }

      public carregarEventos(): void {
        this.spinner.show();

        this.eventoService.getEventos(this.pagination.currentPage,
          this.pagination.itemsPerPage).subscribe(
            (paginatedResult: PaginatedResult<Evento[]>) => {
              this.eventos = paginatedResult.result;
              this.pagination = paginatedResult.pagination;
            },
            (error: any) => {
              this.spinner.hide();
              this.toastr.error('Erro ao Carregar os Eventos', 'Erro!');
            }
            )
            .add(() => this.spinner.hide());
          }

          openModal(event: any, template: TemplateRef<any>, eventoId: number): void {
            event.stopPropagation(); // Faz com que ao clicar na grid ele não entre no atributo
            this.eventoId = eventoId;
            this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
          }

          public pageChanged(event): void {
            this.pagination.currentPage = event.page;
            this.carregarEventos();
          }

          confirm(): void {
            this.modalRef.hide();
            this.spinner.show();

            this.eventoService.deleteEvento(this.eventoId).subscribe(
              (result: any) => {
                if (result.message === 'Deletado') {
                  this.toastr.success('O Evento foi deletado com Sucesso.', 'Deletado!');
                  this.carregarEventos();
                }
              },
              (error: any) => {
                console.error(error);
                this.toastr.error(`Erro ao tentar deletar o evento ${this.eventoId}`, 'Erro');
              }
              ).add(() => this.spinner.hide());
            }

            decline(): void {
              this.modalRef.hide();
            }

            detalheEvento(id: number): void{
              this.router.navigate([`eventos/detalhe/${id}`]);
            }
          }
