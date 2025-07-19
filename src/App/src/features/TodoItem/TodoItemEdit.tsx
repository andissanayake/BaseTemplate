import React, { useEffect } from "react";
import { Modal, Form, Input, DatePicker, Select, notification } from "antd";
import { useTodoItemStore } from "./todoItemStore";
import { apiClient } from "../../common/apiClient";
import dayjs from "dayjs";
import { useParams } from "react-router-dom";
import { TodoItem, PriorityLevel } from "./TodoItemModel";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

interface TodoItemEditProps {
  visible: boolean;
  onClose: () => void;
  todoItem: TodoItem;
  refresh: () => void;
}

const TodoItemEdit: React.FC<TodoItemEditProps> = ({
  visible,
  onClose,
  todoItem,
  refresh,
}) => {
  const [form] = Form.useForm();
  const { setLoading } = useTodoItemStore();
  const { listId } = useParams();

  const getPriorityLevels = () => {
    const priorityOptions = [
      { label: "None", value: PriorityLevel.None },
      { label: "Low", value: PriorityLevel.Low },
      { label: "Medium", value: PriorityLevel.Medium },
      { label: "High", value: PriorityLevel.High },
    ];
    return priorityOptions;
  };

  useEffect(() => {
    if (todoItem) {
      form.setFieldsValue({
        ...todoItem,
        reminder: todoItem.reminder ? dayjs(todoItem.reminder) : undefined,
      });
    }
  }, [todoItem, form]);

  const handleSave = async () => {
    form.validateFields().then(async (values) => {
      const payload: TodoItem = {
        ...values,
        reminder: values.reminder ? dayjs(values.reminder).format() : null,
        id: todoItem.id,
        listId: +listId!,
      };
      setLoading(true);
      apiClient.put<boolean>(`/api/todoItems/${payload.id}`, payload, {
        onSuccess: () => {
          notification.success({
            message: "Todo item updated successfully!",
          });
          refresh();
          onClose();
        },
        onValidationError: (errors) => {
          handleFormValidationErrors({
            form,
            errors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to update todo item!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  return (
    <Modal
      title="Edit Todo Item"
      open={visible}
      onCancel={onClose}
      onOk={handleSave}
    >
      <Form form={form} layout="vertical">
        <Form.Item label="Title" name="title" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item label="Note" name="note">
          <Input.TextArea rows={3} />
        </Form.Item>
        <Form.Item label="Reminder" name="reminder">
          <DatePicker
            showTime
            format="YYYY-MM-DD HH:mm"
            style={{ width: "100%" }}
          />
        </Form.Item>
        <Form.Item label="Priority" name="priority">
          <Select options={getPriorityLevels()} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoItemEdit;
