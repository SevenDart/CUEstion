import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Question} from '../Models/Question';
import {Answer} from '../Models/Answer';
import {AppComment} from '../Models/AppComment';

@Injectable()
export class QuestionsService {
  private _serverAddress = 'https://localhost:44376';

  constructor(private http: HttpClient) {
  }

  SearchQuestions(query: string, tags: string[]) {
    let url = this._serverAddress + '/questions/search' + `?query=${query}`;
    for (const tag of tags) {
      url += `&tags=${encodeURIComponent(tag)}`;
    }
    return this.http.get<Question[]>(url);
  }

  GetHotQuestions(count: number) {
    return this.http.get<Question[]>(this._serverAddress + '/questions/hot' + `?count=${count}`);
  }

  GetQuestion(id: number) {
    return this.http.get<Question>(this._serverAddress + `/questions/${id}`);
  }

  GetAnswersForQuestion(id: number) {
    return this.http.get<Answer[]>(this._serverAddress + `/questions/${id}/answers`);
  }

  GetCommentsForQuestion(id: number) {
    return this.http.get<AppComment[]>(this._serverAddress + `/questions/${id}/comments`);
  }

  GetCommentsForAnswer(questionId: number, answerId: number) {
    return this.http.get<AppComment[]>(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments`);
  }

  GetAllTags() {
    return this.http.get<string[]>(this._serverAddress + '/questions/tags');
  }

  CreateQuestion(header: string, description: string, tags: string[]) {
    const question = {
      header: header,
      text: description,
      tags: tags,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.post(this._serverAddress + '/questions', question, options);
  }

  UpdateQuestion(id: number, description: string, tags: string[]) {
    const question = {
      id: id,
      text: description,
      tags: tags,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.put(this._serverAddress + `/questions/${id}`, question, options);
  }

  DeleteQuestion(questionId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.delete(this._serverAddress + `/questions/${questionId}`, options);
  }

  AddAnswer(questionId: number, text: string) {
    const answer = {
      text: text,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.post(this._serverAddress + `/questions/${questionId}/answers`, answer, options);
  }

  UpdateAnswer(questionId: number, answerId: number, text: string) {
    const answer = {
      id: answerId,
      text: text,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}`, answer, options);
  }

  DeleteAnswer(questionId: number, answerId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.delete(this._serverAddress + `/questions/${questionId}/answers/${answerId}`, options);
  }

  CreateComment(questionId: number, answerId: number, text: string) {
    const comment = {
      text: text,
      questionId: questionId,
      answerId: answerId,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    if (answerId != null) {
      return this.http.post(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments`, comment, options);
    } else {
      return this.http.post(this._serverAddress + `/questions/${questionId}/comments`, comment, options);
    }
  }

  UpdateComment(questionId: number, answerId: number, commentId: number, text: string) {
    const comment = {
      id: commentId,
      text: text,
      user: {
        id: Number(localStorage.getItem('userId'))
      }
    };

    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    if (answerId != null) {
      return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments/${commentId}`, comment, options);
    } else {
      return this.http.put(this._serverAddress + `/questions/${questionId}/comments/${commentId}`, comment, options);
    }
  }

  DeleteComment(questionId: number, answerId: number, commentId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    if (answerId != null) {
      return this.http.delete(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments/${commentId}`, options);
    } else {
      return this.http.delete(this._serverAddress + `/questions/${questionId}/comments/${commentId}`, options);
    }
  }

  UpvoteElement(questionId: number, answerId: number, commentId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    if (commentId != null) {
      if (answerId != null) {
        return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments/${commentId}/upvote`,
          null, options);
      } else {
        return this.http.put(this._serverAddress + `/questions/${questionId}/comments/${commentId}/upvote`, null, options);
      }
    } else if (answerId != null) {
      return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}/upvote`, null, options);
    } else {
      return this.http.put(this._serverAddress + `/questions/${questionId}/upvote`, null, options);
    }
  }

  DownvoteElement(questionId: number, answerId: number, commentId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    if (commentId != null) {
      if (answerId != null) {
        return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}/comments/${commentId}/downvote`,
          null, options);
      } else {
        return this.http.put(this._serverAddress + `/questions/${questionId}/comments/${commentId}/downvote`, null, options);
      }
    } else if (answerId != null) {
      return this.http.put(this._serverAddress + `/questions/${questionId}/answers/${answerId}/downvote`, null, options);
    } else {
      return this.http.put(this._serverAddress + `/questions/${questionId}/downvote`, null, options);
    }
  }

  CreateTag(tag: string) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    tag = encodeURIComponent(tag);
    return this.http.post(encodeURI(this._serverAddress + `/questions/tags?tag=${tag}`), null, options);
  }

  UpdateTag(oldTag: string, newTag: string) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    oldTag = encodeURIComponent(oldTag);
    newTag = encodeURIComponent(newTag);
    return this.http.put(this._serverAddress + `/questions/tags?oldTag=${oldTag}&newTag=${newTag}`, null, options);
  }

  DeleteTag(tag: string) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    tag = encodeURIComponent(tag);
    return this.http.delete(this._serverAddress + `/questions/tags?tag=${tag}`, options);
  }

  SubscribeToQuestion(questionId: number) {
    const options = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.get(this._serverAddress + `/questions/${questionId}/subscribe`, options);
  }
}
