import { Component, Inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

/**
 * MailGmail frontend
 */
@Component({
  selector: 'app-root',
  templateUrl: './mail.component.html'
})
export class MailComponent {

  public to: string;
  public bodyModel: string;
  public subjectModel: string;
  public model: SendMailModel;
  public errorMessage: string;
  public lastResult: string;

  public findModel: FindModel;
  public errorFindMessage: string;
  public lastFindResult: string;

  private _http: HttpClient;
  private _baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
    this.model = {
      clientMsgId: '',
      from: '',
      to: '',
      bodyTemplate: 0,
      subjectModel: {},
      bodyModel: {},
      expiration: new Date(Date.now())
    };

    this.findModel = {
      messageId: null,
      clientMsgId: null,
      status: null
    };
  }

  /**
   * Sends a message
   */
  public send() {
    this.lastResult = null;
    this.errorMessage = null;
    this.model.bodyModel = null;
    this.model.subjectModel = null;
    try {
      if (this.bodyModel) {
        this.model.bodyModel = JSON.parse(this.bodyModel);
      }

      if (this.subjectModel) {
        this.model.subjectModel = JSON.parse(this.subjectModel);
      }

      this._http.post<EmailStatus>(this._baseUrl + 'api/Mail/Send', this.model).subscribe(result => {
        this.lastResult = JSON.stringify(result);
      }, error => this.errorMessage = JSON.stringify(error));
    } catch (e) {
      this.errorMessage = e;
    }
  }

  /**
   * Finds messages
   */
  public find() {
    this.lastFindResult = null;
    this.errorFindMessage = null;
    try {
      this._http.post<any>(this._baseUrl + 'api/Mail/FindMessages', this.findModel).subscribe(result => {
        this.lastFindResult = JSON.stringify(result);
      }, error => this.errorFindMessage = JSON.stringify(error));
    } catch (e) {
      this.errorFindMessage = e;
    }
  }
}

interface FindModel {
  messageId?: number;
  clientMsgId?: string;
  status?: string;
}

interface EmailStatus {
  messageId: number;
  clientMsgId: string;
  status: Object;
}

interface SendMailModel {
  clientMsgId: string;
  from: string;
  to: string;
  bodyTemplate: number;
  subjectModel: Object;
  bodyModel: Object;
  expiration: Date;
}
