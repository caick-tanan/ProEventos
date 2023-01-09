import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.scss']
})
export class EventosComponent implements OnInit {

  public eventos: any;

  constructor(private htttp: HttpClient) { }

  ngOnInit(): void {
    this.GetEventos();
  }

  public GetEventos(): void {
    this.htttp.get('https://localhost:5001/api/eventos').subscribe(
      response => this.eventos = response,
      error => console.log(error),
    );
  }

}
